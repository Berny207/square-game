using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMPexeso.Model
{
	internal class GameBoard
	{
		Square[,] board;
		public int Size;
		private IReadOnlyList<Position> directions = new List<Position>
		{
			new Position(0, 1),  // Up
			new Position(1, 0),  // Right
			new Position(0, -1), // Down
			new Position(-1, 0)  // Left
		};
		public GameBoard(int size)
		{
			board = new Square[size, size];
			Size = size;
			for (int x = 0; x < size; x++)
			{
				for(int y = 0; y < size; y++)
				{
					board[x, y] = new Square(new Position(x, y));
				}
			}
		}
		public Square GetSquare(Position pos)
		{
			return board[pos.X, pos.Y];
		}
		public void SetSquare(Position pos, Square square)
		{
			board[pos.X, pos.Y] = square;
		}
		public List<Square> GetNeighbours(Position position)
		{
			List<Square> neighbours = new List<Square>();
			foreach (Position direction in directions)
			{
				Position newPosition = position + direction;
				if (newPosition.X >= 0 && newPosition.X < board.GetLength(0) &&
					newPosition.Y >= 0 && newPosition.Y < board.GetLength(1))
				{
					neighbours.Add(GetSquare(newPosition));
				}
			}
			return neighbours;
		}
		public bool IsUncontested(Position pos, Player player)
		{
			bool[,] visited = new bool[this.Size, this.Size];
			Queue<Square> squareQueue = new Queue<Square>();
			squareQueue.Enqueue(GetSquare(pos));
			while (squareQueue.Count > 0)
			{
				Square currentSquare = squareQueue.Dequeue();
				Position currentPos = currentSquare.Position;
				List<Square> neighbours = GetNeighbours(currentPos);
				foreach (Square neighbour in neighbours)
				{
					if (neighbour.Owner == player)
					{
						continue;
					}
					if (neighbour.Owner is not null)
					{
						return false;
					}
					Position neighbourPos = neighbour.Position;
					if (visited[neighbourPos.X, neighbourPos.Y])
					{
						continue;
					}
					visited[neighbourPos.X, neighbourPos.Y] = true;
					squareQueue.Enqueue(neighbour);
				}
			}
			return true;
		}
		public void FloodFill(Position pos, Player player)
		{
            Queue<Square> squareQueue = new Queue<Square>();
            squareQueue.Enqueue(GetSquare(pos));
            while (squareQueue.Count > 0)
            {
                Square currentSquare = squareQueue.Dequeue();
                currentSquare.changeOwner(player);
                Position currentPos = currentSquare.Position;
                List<Square> neighbours = GetNeighbours(currentPos);
                foreach (Square neighbour in neighbours)
                {
                    if (neighbour.Owner is not null)
                    {
						continue;
                    }
					Console.WriteLine($"{neighbour.Position.X}, {neighbour.Position.Y}");
					squareQueue.Enqueue(neighbour);
                }
            }
        }
	}
}
