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
		}
		public override Task<Position> TakeTurn(GameBoard gameBoard)
		{
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
