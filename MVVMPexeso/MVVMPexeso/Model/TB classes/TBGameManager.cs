using MVVMPexeso.Model.Core_classes;
using MVVMPexeso.Model.Core_interfaces;
using MVVMPexeso.Model.Enums;
using System.Reflection;
using System.Text;
using System.Windows.Media;

namespace MVVMPexeso.Model.TB_classes
{
	internal class TBGameManager : GameManager
	{
		TBPlayer? currentPlayer;
		protected TBGameBoard TBBoard 
		{ 
			get 
			{
				if (Board is null)
				{
					throw new Exception("Tried using null Board value.");
				}
				if ((TBGameBoard)Board is null)
				{
					throw new Exception("Tried parsing IGameBoard as TBGameBoard");
				}
				return (TBGameBoard)Board; 
			} 
		}
		public TBGameManager(int gridSize, int playerCount)
		{
			TurnOrder = new List<IPlayer>();
			PreparePlayers(playerCount);
			Board = new TBGameBoard(gridSize);
		}
		public override void Game()
		{
			IsGameRunning = true;
			if(Board is null)
			{
				return;
			}
			currentPlayer = (TBPlayer)TurnOrder[0];
			foreach(TBPlayer player in TurnOrder)
			{
				bool result = SelectStartPosition(player);
				if(result == false)
				{
					EndGame("Game creation failed.");
					return;
				}
				if(player is TBHumanPlayer)
					UpdateUI(player);
			}
			int skipCounter = 0;
			while (GetGameRunning())
			{
				if(skipCounter >= TurnOrder.Count)
				{
					EndGame(GameResults());
				}
				if (currentPlayer.GetPossibleMoves().Count == 0)
				{
					NextPlayer();
					skipCounter++;
					continue;
				}
				skipCounter = 0;
				if (currentPlayer is TBAIPlayer)
				{
					TBAIPlayer AIPlayer = (TBAIPlayer)currentPlayer;
					ISquare move = AIPlayer.CalculateTurn(this);
					DoPlayerTurn(AIPlayer, move);
					NextPlayer();
				}
				else
				{
					CheckInputRequest();
				}
			}
		}
		private void NextPlayer()
		{
			if(currentPlayer is null)
			{
				currentPlayer = (TBPlayer)TurnOrder[0];
			}
			int index = TurnOrder.IndexOf(currentPlayer);
			currentPlayer = (TBPlayer)TurnOrder[(index + 1) % TurnOrder.Count];
		}
		void CheckInputRequest()
		{
			var square = PopUserInput();
			if (square is not null)
			{
				SquareClicked(square);
			}
		}
		bool SelectStartPosition(IPlayer player)
		{
			ISquare? square = null;
			try
			{
				square = TBBoard.GetRandomEmptySquare();
			}
			catch
			{
				return false;
			}
			SetSquareOwner(square, player);
			return true;
		}
		private void PreparePlayers(int playerCount)
		{
			PlayerColors playerColours = new PlayerColors();
			for (int i = 0; i < playerCount; i++)
			{
				if(i == 0)
				{
					TurnOrder.Add(new TBHumanPlayer(Colors.Blue));
					continue;
				}
				TurnOrder.Add(new TBAIPlayer(playerColours.PickRandomColor(), this));
			}
			Shuffle(TurnOrder);
		}
		private void SetSquareOwner(ISquare square, IPlayer player)
		{
			square.SetOwner(player);
			square.SetColor(player.GetColor());
			player.AddSquare(square);
			UpdatePlayerPossibleMoves(player, square);
			if(player is TBHumanPlayer)
			{
				UpdateUI(player);
			}
		}
		public override void SquareClicked(ISquare square)
		{
			if(currentPlayer is not TBHumanPlayer)
			{
				return;
			}
			// process player's input
			bool turnResult = false;
			turnResult = DoPlayerTurn(currentPlayer, square);
			if(!turnResult)
			{
				return;
			}
			NextPlayer();
		}
		private bool DoPlayerTurn(IPlayer player, ISquare square)
		{
			// is turn correct?
			if(!player.IsSquarePossibleMove(square))
			{
				return false;
			}
			SetSquareOwner(square, player);
			DoAutoFill(player, square);
			return true;
		}
		private void UpdatePlayerPossibleMoves(IPlayer player, ISquare square)
		{
			player.RemovePossibleMove(square);
			List<ISquare> neighbours = TBBoard.GetNeighbours(square.GetPosition());
			foreach(ISquare neighbour in neighbours)
			{
				IPlayer? neighbourOwner = neighbour.GetOwner();
				if (neighbourOwner is null)
				{
					if (!player.IsSquarePossibleMove(neighbour)) 
					{
						player.AddPossibleMove(neighbour);
					}
				}
				else if(neighbourOwner != player)
				{
					neighbourOwner.RemovePossibleMove(square);
				}
			}
		}
		private void DoAutoFill(IPlayer player, ISquare square)
		{
			List<ISquare> neighbours = Board.GetNeighbours(square.GetPosition());
			foreach (TBSquare neighbour in neighbours)
			{
				if (neighbour.Owner is not null)
				{
					continue;
				}
				bool uncontested = TBBoard.IsUncontested(neighbour, player);
				if (!uncontested)
				{
					continue;
				}
				List<ISquare> squaresToUpdate = TBBoard.FloodFill(neighbour, player);
				foreach (ISquare toUpdate in squaresToUpdate)
				{
					SetSquareOwner(toUpdate, player);
				}
			}
		}
		private void Shuffle<T>(IList<T> list)
		{
			Random rng = new Random();
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				(list[n], list[k]) = (list[k], list[n]); // swap
			}
		}
		private string GetColorName(Color color)
		{
			var colorProperties = typeof(Colors)
				.GetProperties(BindingFlags.Public | BindingFlags.Static);

			foreach (var prop in colorProperties)
			{
				var knownColor = (Color)prop.GetValue(null);
				if (knownColor.Equals(color))
					return prop.Name;
			}

			return color.ToString(); // fallback: #AARRGGBB
		}
		private String GameResults()
		{
			StringBuilder sb = new StringBuilder();
			foreach (Player player in TurnOrder)
			{
				int playerScore = player.GetOwnedSquares().Count;
				sb.Append($"{GetColorName(player.GetColor())} got {playerScore} squares. That is {Math.Round(playerScore / Math.Pow(Board.GetSize(), 2)*100)}% of all squares.\n");
			}
			return sb.ToString();
		}
	}
}
