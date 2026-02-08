using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMPexeso.Model.Core_interfaces
{
	internal interface IGameBoard
	{
		internal abstract List<ISquare> GetNeighbours(Position position);
		internal abstract ISquare GetSquare(Position position);
		internal abstract void SetSquare(Position position, ISquare square);
		internal abstract int GetSize();
		internal abstract void ClearBoard();
	}
}
