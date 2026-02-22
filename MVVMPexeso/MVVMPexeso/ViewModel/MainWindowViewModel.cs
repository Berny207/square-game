using MVVMPexeso.Model;
using MVVMPexeso.Model.Core_classes;
using MVVMPexeso.Model.Core_interfaces;
using MVVMPexeso.Model.RTS_classes;
using MVVMPexeso.Model.TB_classes;
using MVVMProject.MVVM;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows;
using System.IO;

namespace MVVMPexeso.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public RelayCommand StartCommand => new RelayCommand(execute => StartGame(), canExecute => gameManager == null || gameManager.IsGameRunning == false);
        public RelayCommand EndCommand => new RelayCommand(execute => gameManager.EndGame(null), canExecute => gameManager != null && gameManager.IsGameRunning == true);
		public RelayCommand SquareClickCommand => new RelayCommand(execute => userClicked((SquareViewModel)execute));
        public RelayCommand SaveCommand => new RelayCommand(execute => SaveGame(), canExecute => gameManager != null && gameManager.IsGameRunning == true);
        public RelayCommand LoadCommand => new RelayCommand(execute => LoadGame(), canExecute => gameManager == null || gameManager.IsGameRunning == false);
        public RelayCommand RulesCommand => new RelayCommand(execute => ShowRules());
		//public RelayCommand SaveCommand => new RelayCommand(execute => StartGame(), canExecute => gameManager == null || gameManager.IsGameRunning == false);
		//public RelayCommand LoadCommand => new RelayCommand(execute => SquareClicked(execute as SquareViewModel), canExecute => gameManager == null || gameManager.IsGameRunning == false);
		public MainWindowViewModel() 
        {
            UISquares = new ObservableCollection<SquareViewModel>();
		}
		private GameManager? gameManager;
        private Color defaultColor = Colors.LightGray;

		#region Data Binding
		// Vlastnosti, na nichž máme data binding: karty pexesa, velikost gridu (neměnné), skóre
		public ObservableCollection<SquareViewModel> UISquares { get; set; }
        public GameBoard? Squares;
		public List<Player> TurnOrder = new List<Player>();
        public int MIN_PLAYERS { get; } = 2;
        public int MAX_PLAYERS { get; } = 9;

        public int MIN_FIELD_SIZE { get; } = 5;
        public int MAX_FIELD_SIZE { get; } = 20;
        private double _score;
		// Vlastnosti, na nichž máme data binding: karty pexesa, velikost gridu (neměnné), skóre
		private bool _isGameNotRunning = true;
        public bool IsGameNotRunning
        {
            get => _isGameNotRunning;
            set
            {
                _isGameNotRunning=value;
                OnPropertyChanged();
            }
        }
		public double Score
        {
            get => _score;
            set
            {
                if (_score != value)
                {
                    _score = value;
                    OnPropertyChanged();
                }
            }
        }
		public int SquareCount { get; set; }
        private int _playerAmount;
		public int PlayerAmount
        {
            get { return _playerAmount; }
            set
            {
                if (MIN_PLAYERS <= value & value <= MAX_PLAYERS)
                {
                    _playerAmount = value;
                    OnPropertyChanged();
                }
                else
                {
                    throw new Exception("Invalid player count");
                }
            }
        }
		private int _gridSize;
        public int GridSize
		{
			get { return _gridSize; }
			set
			{
                if (gameManager is not null && gameManager.IsGameRunning)
                    return;

				if (MIN_FIELD_SIZE <= value & value <= MAX_FIELD_SIZE)
				{
					_gridSize = value;
					OnPropertyChanged();
				}
				else
				{
					throw new Exception("Invalid grid size");
				}
			}
		}
        private int _boardSize;
        public int BoardSize
        {
            get => _boardSize;
            set
            {
                if (_boardSize == value) return;
                _boardSize = value;

                GridSize = value;
            }
        }
        private int _playerCount;
        public int PlayerCount
        {
            get => _playerCount;
            set
            {
                if (_playerCount == value) return;
                _playerCount = value;

                if (IsGameNotRunning)
                    SetPlayerCount(value);
            }
        }
        private double _capacity;
        public double Capacity
        {
            get => _capacity;
            set
            {
                if (_capacity == value) return;
                _capacity = value;
                OnPropertyChanged();
            }
		}

		private double _army;
        public double Army
        {
            get => _army;
            set
            {
                if (_army == value) return;
                _army = value;
                OnPropertyChanged();
            }
        }
		private bool _rtsMode;
		public bool RTSMode
		{
			get => _rtsMode;
			set
			{
				if (_rtsMode == value) return;
				_rtsMode = value;
				OnPropertyChanged();
			}
		}
		#endregion
		public void UpdateUI(IPlayer playerI)
		{
			if (RTSMode)
			{
				RTSPlayer player = (RTSPlayer)playerI;
				Army = Math.Round(player.GetArmy(), 2);
                Capacity = player.GetCapacity();
			}
			Score = Math.Round(playerI.GetOwnedSquares().Count / Math.Pow(GridSize, 2) * 100, 2);
		}
		public void userClicked(SquareViewModel square)
        {
            if (gameManager is null)
            {
                return;
            }
            gameManager.ProcessInput(square.Model);
		}

        public void SetPlayerCount(int count)
        {
            PlayerAmount = count;
        }
        // Samotná herní logika
        public void StartGame()
        {
            if (BoardSize < MIN_FIELD_SIZE)
            {
                BoardSize = MIN_FIELD_SIZE;
            }
            if (PlayerAmount < MIN_PLAYERS)
            {
                PlayerAmount = MIN_PLAYERS;
            }
            UISquares.Clear();
            GridSize = BoardSize;
            if (RTSMode)
            {
				gameManager = new RTSGameManager(GridSize, PlayerAmount);
            }
            else
            {
                gameManager = new TBGameManager(GridSize, PlayerAmount);
            }
            gameManager.AddUpdateUIHandler(UpdateUI);
            gameManager.AddUpdateGameStateHandler(UpdateGameState);
            createSquares(gameManager.GetGameBoard());
            Thread gameThread = new Thread(gameManager.Game);
            gameThread.Start();
        }
        public void SaveGame()
        {
            MessageBox.Show("Toto měl udělat Monkey D. Negro");
		}
        public void LoadGame()
        {
			MessageBox.Show("Toto měl udělat Monkey D. Negro");
		}
        public void UpdateGameState(bool gameState)
        {
            IsGameNotRunning = !gameState;
        }
        private void createSquares(IGameBoard board)
        {
            for(int i = 0; i < GridSize; i++)
            {
                for(int j = 0; j < GridSize; j++)
                {
                    var squareVM = new SquareViewModel(board.GetSquare(new Position(i, j)), defaultColor);
					UISquares.Add(squareVM);
				}
            }
		}
        public void ShowRules()
        {
            MessageBox.Show("Non RTS gamemode:\r\n\teach player takes turn\r\n\tduring player's turn, player HAS to capture neighbouring square\r\n\thuman player is always blue\r\n\thuman player's possible moves are highlighted\r\n\tthe goal is to own as many squares as possible\r\n\r\nRTS gamemode:\r\n\tlock in");
        }
	}
}