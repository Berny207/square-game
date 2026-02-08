using MVVMPexeso.Model.TB_classes;
using System.Windows.Media;


namespace MVVMPexeso.Model
{
	internal class TBHumanPlayer : TBPlayer
	{
		public TBHumanPlayer(Color playerColor)
		{
			this.SetColor(playerColor);
			this.SetScore(0);
		}

	}
}
