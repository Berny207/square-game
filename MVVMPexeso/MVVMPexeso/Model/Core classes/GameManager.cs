using MVVMPexeso.Model.Core_interfaces;

namespace MVVMPexeso.Model.Core_classes
{
	internal abstract class GameManager : IGameManager
	{
		protected event IGameManager.UpdateScoreHandler ScoreUpdated;
		protected IGameBoard Board;
		protected List<IPlayer> TurnOrder;
		protected bool _isGameRunning = false;
		private object _mtLock = new object();
		protected ISquare? userInputCache = null;
		public GameManager()
		{
		}

		public void EndGame()
		{
			IsGameRunning = false;
		}

		public bool IsGameRunning
		{
			get
			{
				lock (_mtLock)
				{
					return _isGameRunning;
				}
			}
			protected set
			{
				lock (_mtLock)
				{
					_isGameRunning = value;
				}
			}
		}

		public void AddUpdateScoreHandler(IGameManager.UpdateScoreHandler handler)
		{
			ScoreUpdated += handler;
		}
		public void UpdateScore(IPlayer player)
		{
			ScoreUpdated.Invoke(player);
		}
		public abstract void Game();

		public void ProcessInput(ISquare square)
		{
			lock (_mtLock)
			{
				userInputCache = square;
			}
		}

		protected ISquare? PopUserInput()
		{
			lock (_mtLock)
			{
				var square = userInputCache;
				userInputCache = null;
				return square;
			}
		}

		public abstract void SquareClicked(ISquare square);
		public bool GetGameRunning()
		{
			return IsGameRunning;
		}
		public List<IPlayer> GetPlayers()
		{
			return TurnOrder;
		}
		public IGameBoard GetGameBoard()
		{
			return Board;
		}
	}
}
