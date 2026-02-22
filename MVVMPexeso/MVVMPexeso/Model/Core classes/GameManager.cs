using MVVMPexeso.Model.Core_interfaces;
using System.Windows;

namespace MVVMPexeso.Model.Core_classes
{
	internal abstract class GameManager : IGameManager
	{
		protected event IGameManager.UpdateUIHandler? UIUpdated;
		protected event IGameManager.UpdateGameStateHandler? GameStateUpdated;
		protected IGameBoard? Board;
		protected List<IPlayer> TurnOrder = new List<IPlayer>();
		protected bool _isGameRunning = false;
		private object _mtLock = new object();
		protected ISquare? userInputCache = null;
		public GameManager()
		{
		}

		public virtual void EndGame(string? message)
		{
			IsGameRunning = false;
			if (message is not null)
			{
				MessageBox.Show(message);
			}
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
					UpdateGameState(value);
				}
			}
		}

		public void AddUpdateUIHandler(IGameManager.UpdateUIHandler handler)
		{
			UIUpdated += handler;
		}
		public void AddUpdateGameStateHandler(IGameManager.UpdateGameStateHandler handler)
		{
			GameStateUpdated += handler;
		}
		public void UpdateUI(IPlayer player)
		{
			if(UIUpdated is null)
			{
				throw new Exception("Tried to update score value without set handler");
			}
			UIUpdated.Invoke(player);
		}
		public void UpdateGameState(bool IsGameRunning)
		{
			if (GameStateUpdated is null)
			{
				throw new Exception("Tried to update score value without set handler");
			}
			GameStateUpdated.Invoke(IsGameRunning);
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
			if (Board is null)
			{
				throw new Exception("Tried returning null Board.");
			}
			return Board;
		}
	}
}
