using MVVMPexeso.Model.Core_classes;
using MVVMPexeso.Model.Core_interfaces;

namespace MVVMPexeso.Model.RTS_classes
{
	internal class RTSGameManager : GameManager
	{
		public RTSGameManager(int gridSize, int playerCount)
		{
			//PreparePlayers(playerCount);
			Board = new TBGameBoard(gridSize);
		}
		public override void Game()
		{

		}
		public override void SquareClicked(ISquare square)
		{
		}
	}
}
