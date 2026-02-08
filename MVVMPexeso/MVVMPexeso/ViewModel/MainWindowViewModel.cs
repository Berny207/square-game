using MVVMPexeso.Model;
using MVVMPexeso.Model.Core_classes;
using MVVMPexeso.Model.Core_interfaces;
using MVVMPexeso.Model.RTS_classes;
using MVVMPexeso.Model.TB_classes;
using MVVMProject.MVVM;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace MVVMPexeso.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public RelayCommand StartCommand => new RelayCommand(execute => StartGame(), canExecute => gameManager == null || gameManager.IsGameRunning == false);
		public RelayCommand EndCommand => new RelayCommand(execute => gameManager.EndGame(), canExecute => gameManager != null && gameManager.IsGameRunning == true);
		public RelayCommand SquareClickCommand => new RelayCommand(execute => userClicked(execute as SquareViewModel));
        public MainWindowViewModel() 
        {
            UISquares = new ObservableCollection<SquareViewModel>();
		}
        private bool _isGameRunning = false;
		private GameManager gameManager;
        private Color defaultColor = Colors.LightGray;
		#region Data Binding
		// Vlastnosti, na nichž máme data binding: karty pexesa, velikost gridu (neměnné), skóre
		public ObservableCollection<SquareViewModel> UISquares { get; set; }
		public int MIN_PLAYERS { get; } = 2;
		public int MAX_PLAYERS { get; } = 9;
		public int MIN_FIELD_SIZE { get; } = 3;
		public int MAX_FIELD_SIZE { get; } = 9;
		private double _score;
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
		public void UpdateScore(IPlayer player)
        {
			Score = Math.Round(player.GetOwnedSquares().Count/Math.Pow(GridSize, 2)*100, 2);
		}
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

                if (!_isGameRunning)
                    SetPlayerCount(value);
            }
        }
        #endregion

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


		#region Herní logika
		// Samotná herní logika
        public void StartGame()
        {
            if(BoardSize < MIN_FIELD_SIZE)
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
			gameManager.AddUpdateScoreHandler(UpdateScore);
			createSquares(gameManager.GetGameBoard());
			Thread gameThread = new Thread(gameManager.Game);
            gameThread.Start();
		}
        private void createSquares(IGameBoard board)
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    var squareVM = new SquareViewModel(board.GetSquare(new Position(i, j)), defaultColor);
                    UISquares.Add(squareVM);
                }
            }
		}
		#endregion
	}
}

    

