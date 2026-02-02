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
    internal abstract class Player
    {
        public int Score;
        public bool IsHuman;
        public List<Square> OwnedSquares = new List<Square>();
        public Color PlayerColor;
        public TaskCompletionSource<Position> tcs;
		abstract public Task<Position> TakeTurn(GameBoard gameBoard);
		abstract public Task<Position> TakeInitialTurn(GameBoard gameBoard);
	}
}
