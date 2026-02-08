using MVVMPexeso.Model.Core_classes;
using MVVMPexeso.Model.Core_interfaces;
using MVVMPexeso.Model.Enums;
using MVVMPexeso.Model.RTS_classes;
using MVVMPexeso.Model.TB_classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace MVVMPexeso.Model
{
	internal class RTSGameBoard : GameBoard
	{
		protected RTSSquare[,] Grid;
		protected List<RTSPlayer> Players;
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

		RTSSquare GetRTSSquare(ISquare square)
		{
			return (RTSSquare)square;
		}

		public List<RTSSquare> GetNeighbours(Position position)
		{
			List<RTSSquare> neighbours = new List<RTSSquare>();
			foreach (Position direction in Directions.DirectionsBase)
			{
				Position newPosition = position + direction;
				if (newPosition.X >= 0 && newPosition.X < Grid.GetLength(0) &&
					newPosition.Y >= 0 && newPosition.Y < Grid.GetLength(1))
				{
					neighbours.Add(GetSquare(newPosition) as RTSSquare);
				}
			}
			return neighbours;
		}
	}
}
