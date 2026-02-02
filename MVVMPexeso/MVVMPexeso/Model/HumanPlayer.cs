using MVVMPexeso.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;


namespace MVVMPexeso.Model
{
	internal class HumanPlayer : Player
	{
		public HumanPlayer(Color playerColor)
		{
			PlayerColor = playerColor;
			IsHuman = true;
			Score = 0;
			CanPlay = true;
		}
		public override Task<Position> TakeTurn(GameBoard gameBoard)
		{
            int gridSize = gameBoard.Size;
            //  get all possible moves from owned squares
            bool[,] availibleMovesLocations = new bool[gridSize, gridSize];
            List<Position> availibleMoves = new List<Position>();
            foreach (Square ownedSquare in OwnedSquares)
            {
                foreach (Square neighbour in gameBoard.GetNeighbours(ownedSquare.Position))
                {
                    if (neighbour.Owner is not null)
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
			if(availibleMoves.Count == 0)
			{
                throw new Exception("No available moves");
            }
            tcs = new TaskCompletionSource<Position>();
			return tcs.Task;
		}
		public override Task<Position> TakeInitialTurn(GameBoard gameBoard)
		{
			tcs = new TaskCompletionSource<Position>();
			return tcs.Task;
		}
		public void SquareClicked(Position position)
		{
			if(tcs == null)
			{
				return;
			}
			// turn check
			tcs.SetResult(position);
			tcs = null;
		}
	}
}
