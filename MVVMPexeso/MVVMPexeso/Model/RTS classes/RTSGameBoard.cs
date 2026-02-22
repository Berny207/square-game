using MVVMPexeso.Model.Core_classes;
using MVVMPexeso.Model.Core_interfaces;
using MVVMPexeso.Model.RTS_classes;

namespace MVVMPexeso.Model
{
	internal class RTSGameBoard : GameBoard
	{
		public RTSGameBoard(int size)
		{
			Grid = new RTSSquare[size, size];
			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					Grid[x, y] = new RTSSquare(new Position(x, y));
				}
			}
		}
		public ISquare GetRandomEmptySquare()
		{
			for (int attempt = 0; attempt < 1000; attempt++)
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
	}
}
