using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static System.Formats.Asn1.AsnWriter;

namespace MVVMPexeso.Model
{
	internal class RTSAIPlayer : RTSPlayer
	{
		public RTSAIPlayer(Color playerColor)
		{
			this.SetColor(playerColor);
			this.SetScore(0);
		}
	}
}
