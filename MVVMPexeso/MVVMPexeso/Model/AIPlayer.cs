using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Numerics;
using MVVMPexeso.ViewModel;

namespace MVVMPexeso.Model
{
	internal class AIPlayer : Player
	{
		public AIPlayer(Color playerColor)
		{
			PlayerColor = playerColor;
			IsHuman = false;
			Score = 0;
			CanPlay = true;
		}
		public override Task<Position> TakeTurn(GameBoard gameBoard)
		{
			Random rng = new Random();
			int gridSize = gameBoard.Size;
			//  get all possible moves from owned squares
			bool[,] availibleMovesLocations = new bool[gridSize, gridSize];
			List<Position> availibleMoves = new List<Position>();
			foreach (Square ownedSquare in OwnedSquares)
			{
				foreach(Square neighbour in gameBoard.GetNeighbours(ownedSquare.Position))
				{
					if(neighbour.Owner is not null)
					{
						continue;
					}
					if (availibleMovesLocations[neighbour.Position.X, neighbour.Position.Y])
					{
						continue;
					}
					availibleMovesLocations[neighbour.Position.X, neighbour.Position.Y] = true;
					availibleMoves.Add(neighbour.Position);
				}
			}
			if (availibleMoves.Count == 0)
			{
				throw new Exception("No available moves");
			}
			return Task.FromResult(availibleMoves[rng.Next(availibleMoves.Count)]);
		}

		public override Task<Position> TakeInitialTurn(GameBoard gameBoard)
		{
			Random rng = new Random();
			int gridSize = gameBoard.Size;
			List<Position> availibleMoves = new List<Position>();
			for (int x = 0; x < gridSize; x++)
			{
				for (int y = 0; y < gridSize; y++)
				{
					Square square = gameBoard.GetSquare(new Position(x, y));
					if (square.Owner is null)
					{
						availibleMoves.Add(square.Position);
					}
				}
			}
			if (availibleMoves.Count == 0)
			{
				throw new Exception("No available moves for AI initial turn");
			}
			return Task.FromResult(availibleMoves[rng.Next(availibleMoves.Count)]);
		}
	}
}
