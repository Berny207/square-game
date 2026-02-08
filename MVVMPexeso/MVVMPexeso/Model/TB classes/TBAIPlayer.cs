using System.Windows.Media;
using MVVMPexeso.Model.TB_classes;
using MVVMPexeso.Model.Core_interfaces;

namespace MVVMPexeso.Model
{
	internal class TBAIPlayer : TBPlayer
	{
		public TBAIPlayer(Color playerColor, TBGameManager gameManager)
		{
			this.SetColor(playerColor);
			this.SetScore(0);
			this.SetGameManager(gameManager);
		}
		public TBSquare CalculateTurn(TBGameManager gameManager)
		{
			Random rng = new Random();
			List<ISquare> possibleMoves = this.GetPossibleMoves();
			TBGameBoard gameBoard = gameManager.GetGameBoard() as TBGameBoard;
			Dictionary<TBPlayer, float> playerTreatLevels = GetPlayerThreatLevels(gameManager);
			TBPlayer biggestThreat = null;
			int maxPlayerThreatLevel = -1;
			foreach (var kvp in playerTreatLevels)
			{
				if (kvp.Key == this)
				{
					continue;
				}
				if (kvp.Value > maxPlayerThreatLevel)
				{
					maxPlayerThreatLevel = (int)kvp.Value;
					biggestThreat = kvp.Key;
				}
			}
			TBSquare bestMove = null;
			int closestDistance = int.MaxValue;
			if(biggestThreat is null)
			{
				return possibleMoves[rng.Next(possibleMoves.Count)] as TBSquare;
			}
			foreach (TBSquare possibleMove in possibleMoves)
			{
				int distance = gameBoard.GetDistanceFromPlayer(possibleMove.GetPosition(), biggestThreat);
				if (distance == -1)
				{
					continue;
				}
				if (distance < closestDistance)
				{
					closestDistance = distance;
					bestMove = possibleMove;
				}
			}
			if (bestMove is not null)
			{
				return bestMove;
			}
			return possibleMoves[rng.Next(possibleMoves.Count)] as TBSquare;
		}

		public Dictionary<TBPlayer, float> GetPlayerThreatLevels(TBGameManager gameManager)
		{
			List<IPlayer> Players = gameManager.GetPlayers();
			TBGameBoard gameBoard = gameManager.GetGameBoard() as TBGameBoard;
			int size = gameBoard.GetSize();
			Dictionary<TBPlayer, float> threatLevels = new Dictionary<TBPlayer, float>();
			foreach (TBPlayer p in Players)
			{
				threatLevels.Add(p, 0);
			}
			// analyze empty fields and their neigbours
			// for each empty field, check it's neighbours. For each player, add threat level by fieldSize*(totalNeighbours/playerNeighbours)
			// so we need - all field sizes and their neighbours
			bool[,] visited = new bool[size, size];
			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					if (visited[x, y])
					{
						continue;
					}
					ISquare selectedSquare = gameBoard.GetSquare(new Position(x, y));
					if (selectedSquare.GetOwner() is not null)
					{
						// Ingore full spaces
						continue;
					}
					// Square is empty, start flood fill to get field size and neighbours
					int fieldSize = 0;
					List<ISquare> neighbouringSquares = new List<ISquare>();
					bool[,] currentVisited = new bool[size, size];
					(fieldSize, neighbouringSquares, currentVisited) = AnalyzeEmptyField(gameBoard, selectedSquare as TBSquare);
					// Flood fill done, time to analyze it's results
					Dictionary<IPlayer, float> threatLevelsForOneField = new Dictionary<IPlayer, float>();
					foreach (IPlayer p in Players)
					{
						threatLevelsForOneField.Add(p, 0);
					}
					foreach (ISquare neighbour in neighbouringSquares)
					{
						IPlayer? neighbourOwner = neighbour.GetOwner();
						if (neighbourOwner is null)
						{
							continue;
						}
						threatLevelsForOneField[neighbourOwner] += 1;
					}
					float myThreatLevelForOneField = threatLevelsForOneField[this];
					foreach (TBPlayer p in Players)
					{
						threatLevels[p] += fieldSize * threatLevelsForOneField[p] / neighbouringSquares.Count * myThreatLevelForOneField / neighbouringSquares.Count;
					}
					// Merge visited arrays
					for (int i = 0; i < size; i++)
					{
						for (int j = 0; j < size; j++)
						{
							if (currentVisited[i, j])
							{
								visited[i, j] = true;
							}
						}
					}
				}
			}
			return threatLevels;
		}
		public (int, List<ISquare>, bool[,]) AnalyzeEmptyField(TBGameBoard gameBoard, TBSquare square)
		{
			int size = gameBoard.GetSize();
			bool[,] visited = new bool[size, size];
			bool[,] emptyVisited = new bool[size, size];
			int fieldSize = 0;
			List<ISquare> neighbouringSquares = new List<ISquare>();
			Queue<ISquare> squareQueue = new Queue<ISquare>();
			squareQueue.Enqueue(square);

			while (squareQueue.Count > 0)
			{
				ISquare currentSquare = squareQueue.Dequeue();
				Position currentPos = currentSquare.GetPosition();
				fieldSize++;
				List<ISquare> neighbours = gameBoard.GetNeighbours(currentPos);
				foreach (ISquare neighbour in neighbours)
				{
					Position neighbourPos = neighbour.GetPosition();
					if (visited[neighbourPos.X, neighbourPos.Y])
					{
						continue;
					}
					if (neighbour.GetOwner() is not null)
					{
						emptyVisited[neighbourPos.X, neighbourPos.Y] = true;
						neighbouringSquares.Add(neighbour);
						visited[neighbourPos.X, neighbourPos.Y] = true;
						continue;
					}
					visited[neighbourPos.X, neighbourPos.Y] = true;
					squareQueue.Enqueue(neighbour);
				}
			}
			return (fieldSize, neighbouringSquares, emptyVisited);
		}
	}
}
