
namespace MVVMPexeso.Model.Core_interfaces
{
	internal interface IGameManager
	{
		public abstract void Game();
		public abstract void EndGame();
		public abstract void ProcessInput(ISquare square);
		public abstract bool GetGameRunning();
		public abstract List<IPlayer> GetPlayers();
		public abstract IGameBoard GetGameBoard();

		delegate void UpdateScoreHandler(IPlayer player);
		public abstract void AddUpdateScoreHandler(UpdateScoreHandler handler);
	}
}
