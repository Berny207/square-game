
namespace MVVMPexeso.Model.Core_interfaces
{
	internal interface IGameManager
	{
		public abstract void Game();
		public abstract void EndGame(string message);
		public abstract void ProcessInput(ISquare square);
		public abstract bool GetGameRunning();
		public abstract List<IPlayer> GetPlayers();
		public abstract IGameBoard GetGameBoard();

		delegate void UpdateUIHandler(IPlayer player);
		delegate void UpdateGameStateHandler(bool IsGameRunning);
		public abstract void AddUpdateUIHandler(UpdateUIHandler handler);
		public abstract void AddUpdateGameStateHandler(UpdateGameStateHandler handler);
	}
}
