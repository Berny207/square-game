using MVVMPexeso.Model.Enums;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMPexeso.Model.Core_interfaces
{
	internal interface IPlayer
	{
		internal abstract void SetScore(int newScore);
		internal abstract int GetScore();
		internal abstract List<ISquare> GetOwnedSquares();
		internal abstract void AddSquare(ISquare newOwnedSquare);
		internal abstract void AddPossibleMove(ISquare possibleMove);
		internal abstract void RemovePossibleMove(ISquare possibleMove);
		internal abstract bool IsMoveLegal(ISquare square);
		internal abstract void SetColor(Color color);
		internal abstract List<ISquare> GetPossibleMoves();
		internal abstract IGameManager GetGameManager();
		internal abstract void SetGameManager(IGameManager gameManager);
		internal Color GetColor();
	}
}
