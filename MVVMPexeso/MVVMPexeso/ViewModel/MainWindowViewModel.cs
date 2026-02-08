using MVVMPexeso.Model;
using MVVMPexeso.Model.Core_classes;
using MVVMPexeso.Model.Core_interfaces;
using MVVMPexeso.Model.RTS_classes;
using MVVMPexeso.Model.TB_classes;
using MVVMProject.MVVM;
using System.Collections.ObjectModel;
<<<<<<< Updated upstream
using System.Windows.Media;
=======
<<<<<<< HEAD
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
<<<<<<< Updated upstream
=======
using static System.Formats.Asn1.AsnWriter;
=======
using System.Windows.Media;
>>>>>>> origin/main
>>>>>>> Stashed changes
>>>>>>> Stashed changes

namespace MVVMPexeso.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
<<<<<<< Updated upstream
        public RelayCommand StartCommand => new RelayCommand(execute => StartGame(), canExecute => gameManager == null || gameManager.IsGameRunning == false);
		public RelayCommand EndCommand => new RelayCommand(execute => gameManager.EndGame(), canExecute => gameManager != null && gameManager.IsGameRunning == true);
		public RelayCommand SquareClickCommand => new RelayCommand(execute => userClicked(execute as SquareViewModel));
=======
<<<<<<< HEAD
        public RelayCommand StartCommand => new RelayCommand(execute => StartGame(), canExecute => _isGameRunning == false);
        public RelayCommand SquareClickCommand => new RelayCommand(execute => SquareClicked(execute as SquareViewModel), canExecute => _isGameRunning == true);

=======
        public RelayCommand StartCommand => new RelayCommand(execute => StartGame(), canExecute => gameManager == null || gameManager.IsGameRunning == false);
		public RelayCommand EndCommand => new RelayCommand(execute => gameManager.EndGame(), canExecute => gameManager != null && gameManager.IsGameRunning == true);
		public RelayCommand SquareClickCommand => new RelayCommand(execute => userClicked(execute as SquareViewModel));
>>>>>>> origin/main
>>>>>>> Stashed changes
        public MainWindowViewModel() 
        {
            UISquares = new ObservableCollection<SquareViewModel>();
		}
        private bool _isGameRunning = false;
<<<<<<< Updated upstream
		private GameManager gameManager;
        private Color defaultColor = Colors.LightGray;
=======
<<<<<<< HEAD
        private bool _isBusy = false;

        private SquareViewModel _firstSelected;
        private SquareViewModel _secondSelected;

        private List<Color> PlayerColors = new List<Color>();
		private int _playerAmount;
		public int SquareCount { get; set; }
>>>>>>> Stashed changes
		#region Data Binding
		// Vlastnosti, na nichž máme data binding: karty pexesa, velikost gridu (neměnné), skóre
		public ObservableCollection<SquareViewModel> UISquares { get; set; }
<<<<<<< Updated upstream
=======
        public GameBoard Squares;
		public List<Player> TurnOrder = new List<Player>();
        public int MIN_PLAYERS { get; } = 2;
        public int MAX_PLAYERS { get; } = 5;

        public int MIN_FIELD_SIZE { get; } = 3;
        public int MAX_FIELD_SIZE { get; } = 9;
        private double _score;
=======
		private GameManager gameManager;
        private Color defaultColor = Colors.LightGray;
		#region Data Binding
		// Vlastnosti, na nichž máme data binding: karty pexesa, velikost gridu (neměnné), skóre
		public ObservableCollection<SquareViewModel> UISquares { get; set; }
>>>>>>> Stashed changes
		public int MIN_PLAYERS { get; } = 2;
		public int MAX_PLAYERS { get; } = 9;
		public int MIN_FIELD_SIZE { get; } = 3;
		public int MAX_FIELD_SIZE { get; } = 9;
		private double _score;
<<<<<<< Updated upstream
=======
>>>>>>> origin/main
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
=======
<<<<<<< HEAD
            // TESTING
            UISquares.Clear();
            TurnOrder.Clear();
            CreateGameSquares();
            CreatePlayers();
            _isGameRunning = true;
<<<<<<< Updated upstream
            GameSequence();
=======
            GameSequence(false, 0);
=======
>>>>>>> Stashed changes
            get => _rtsMode;
            set
            {
                if (_rtsMode == value) return;
                _rtsMode = value;
                OnPropertyChanged();
            }
<<<<<<< Updated upstream
=======
>>>>>>> origin/main
>>>>>>> Stashed changes
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
				gameManager = new RTSGameManager(GridSize, PlayerAmount);
=======
<<<<<<< HEAD
                if (i == 0)
                {
                    HumanPlayer = new HumanPlayer(Colors.Blue);
					tempOrder.Add(HumanPlayer);
					continue;
                }
				Color randomColor = PlayerColors[i-1];
				tempOrder.Add(new AIPlayer(randomColor));
            }

            // shuffle turn order
            for (int i = 0; i < PlayerAmount; i++)
            {
                Player randomPlayer = tempOrder[random.Next(tempOrder.Count)];
                tempOrder.Remove(randomPlayer);
                TurnOrder.Add(randomPlayer);
            }
        }

        private async Task GameSequence()
        {
			// Hlavní herní smyčka
			// První tahy nepotřebují kontrolu sousedství
			foreach (Player currentPlayer in TurnOrder)
			{
				await StartingPlayerTurn(currentPlayer);
>>>>>>> Stashed changes
			}
            else
            {
<<<<<<< Updated upstream
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
=======
                int skipCounter = 0;
                foreach (Player currentPlayer in TurnOrder)
                {
                    if (!currentPlayer.CanPlay)
                    {
                        skipCounter++;
                        continue;
                    }
                    await PlayerTurn(currentPlayer);
				}
                if(skipCounter == PlayerAmount)
                {
                    _isGameRunning = false;
                }
			}
=======
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
>>>>>>> origin/main
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
<<<<<<< HEAD
        private bool WeakMoveValidityCheck(Position movePosition)
        {
            Square chosenSquare = Squares.GetSquare(movePosition);
            if (chosenSquare.Owner is not null)
            {
                return false;
            }
            return true;
        }
		private bool MoveValidityCheck(Position movePosition, Player player)
        {
            Square chosenSquare = Squares.GetSquare(movePosition);
            if (!WeakMoveValidityCheck(movePosition))
            {
                return false;
			}
			foreach (Square ownedSquare in player.OwnedSquares)
            {
                List<Square> neighbours = Squares.GetNeighbours(ownedSquare.Position);
                foreach (Square neighbour in neighbours)
                {
					Position position = neighbour.Position;
                    if (movePosition.X == position.X & movePosition.Y == position.Y)
                    {
                        return true;
					}
				}
			}
            return false;
		}
		private void SquareClicked(SquareViewModel clicked)
        {
			HumanPlayer.SquareClicked(clicked.Model.Position);
		}
        #endregion

        public static void Shuffle<T>(IList<T> list)
        {
            Random rnd = new Random();

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
	}

=======
		#endregion
	}
>>>>>>> origin/main
>>>>>>> Stashed changes
}

    

