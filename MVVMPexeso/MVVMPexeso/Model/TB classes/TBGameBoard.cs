using MVVMPexeso.Model.Core_classes;
using MVVMPexeso.Model.Core_interfaces;
using MVVMPexeso.Model.TB_classes;

namespace MVVMPexeso.Model
{
	internal class TBGameBoard : GameBoard
	{
		public TBGameBoard(int size)
		{
			Grid = new TBSquare[size, size];
			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					Grid[x, y] = new TBSquare(new Position(x, y));
				}
			}
		}
		public ISquare GetRandomEmptySquare()
		{
			for(int attempt = 0; attempt < 1000; attempt++)
			{
				int x = Random.Shared.Next(GetSize());
				int y = Random.Shared.Next(GetSize());
				if (Grid[x, y].GetOwner() is null)
				{
					return Grid[x, y];
				}
			}
			throw new Exception("Failed to find an empty square after 1000 attempts.");
		}
		public bool IsUncontested(ISquare square, IPlayer player)
		{
			bool[,] visited = new bool[Grid.Length, Grid.Length];
			Queue<ISquare> squareQueue = new Queue<ISquare>();
			squareQueue.Enqueue(square);
			while (squareQueue.Count > 0)
			{
				ISquare currentSquare = squareQueue.Dequeue();
				Position currentPos = currentSquare.GetPosition();
				List<ISquare> neighbours = GetNeighbours(currentPos);
				foreach (ISquare neighbour in neighbours)
				{
					if (neighbour.GetOwner() == player)
					{
						continue;
					}
					if (neighbour.GetOwner() is not null)
					{
						return false;
					}
					Position neighbourPos = neighbour.GetPosition();
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
		public List<ISquare> FloodFill(ISquare square, IPlayer player)
		{
			bool[,] visited = new bool[GetSize(), GetSize()];
			Queue<ISquare> squareQueue = new Queue<ISquare>();
			List<ISquare> output = new List<ISquare>();
            squareQueue.Enqueue(square);
			int i = 0;
			while (squareQueue.Count > 0)
            {
				i++;
                ISquare currentSquare = squareQueue.Dequeue();
                output.Add(currentSquare);
				Position currentPos = currentSquare.GetPosition();
                List<ISquare> neighbours = GetNeighbours(currentPos);
				visited[currentPos.X, currentPos.Y] = true;
				foreach (ISquare neighbour in neighbours)
                {
					Position neighbourPos = neighbour.GetPosition();
					if (neighbour.GetOwner() is not null)
                    {
						continue;
                    }
					if(visited[neighbourPos.X, neighbourPos.Y])
					{
						continue;
					}
					visited[neighbourPos.X, neighbourPos.Y] = true;
					squareQueue.Enqueue(neighbour);
                }
            }
			Console.WriteLine($"FloodFill visited {i} squares.");
			return output;
        }
		public int GetDistanceFromPlayer(Position pos, TBPlayer player)
		{
			bool[,] visited = new bool[GetSize(), GetSize()];
			PriorityQueue<Position, int> squareQueue = new PriorityQueue<Position, int>();
			squareQueue.Enqueue(pos, 0);
			visited[pos.X, pos.Y] = true;
			while (squareQueue.Count > 0)
			{
				squareQueue.TryDequeue(out Position currentPos, out int distance);
				ISquare currentSquare = GetSquare(currentPos);
				if (currentSquare.GetOwner() == player)
				{
					return distance;
				}
				List<ISquare> neighbours = GetNeighbours(currentSquare.GetPosition());
				foreach (ISquare neighbour in neighbours)
				{
					Position neighbourPos = neighbour.GetPosition();
					if (!visited[neighbourPos.X, neighbourPos.Y])
					{
						visited[neighbourPos.X, neighbourPos.Y] = true;
						squareQueue.Enqueue(neighbourPos, distance + 1);
					}
				}
			}
			return -1; // Player not found
		}
	}
}
