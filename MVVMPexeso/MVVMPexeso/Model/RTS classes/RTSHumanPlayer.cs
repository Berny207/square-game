using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMPexeso.Model
{
	internal class RTSHumanPlayer : RTSPlayer
	{
		public RTSHumanPlayer(Color playerColor)
		{
			this.SetColor(playerColor);
			this.SetScore(0);
		}
	}
}
