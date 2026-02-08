using MVVMPexeso.Model.Core_interfaces;
using MVVMPexeso.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MVVMPexeso.Model.Core_classes
{
	internal class GameBoard : IGameBoard
	{
		internal ISquare[,] Grid;
		public void SetSquare(Position position, ISquare square)
		{
			Grid[position.X, position.Y] = square;
		}
		public ISquare GetSquare(Position position)
		{
			return Grid[position.X, position.Y];
		}

		public List<ISquare> GetNeighbours(Position position)
		{
			List<ISquare> neighbours = new List<ISquare>();
			foreach (Position direction in Directions.DirectionsBase)
			{
				Position newPosition = position + direction;
				if (newPosition.X >= 0 && newPosition.X < Grid.GetLength(0) &&
					newPosition.Y >= 0 && newPosition.Y < Grid.GetLength(1))
				{
					neighbours.Add(GetSquare(newPosition));
				}
			}
			return neighbours;
		}
		public int GetSize()
		{
			return Grid.GetLength(0);
		}

		public void ClearBoard()
		{
			for (int i = 0; i < Grid.GetLength(0); i++)
			{
				for (int j = 0; j < Grid.GetLength(1); j++)
				{
					Grid[i, j].Clear();
				}
			}
		}
	}
}
