using MVVMPexeso.Model;
using MVVMProject.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MVVMPexeso.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public RelayCommand StartCommand => new RelayCommand(execute => StartGame(), canExecute => _isGameRunning == false);
        public RelayCommand SquareClickCommand => new RelayCommand(execute => SquareClicked(execute as SquareViewModel), canExecute => _isGameRunning == true);
        public RelayCommand PlayerCountChangedCommand => new RelayCommand(execute => {
            if (execute is double newSize)
            {
                SetPlayerCount((int)Math.Round(newSize));
            }
        }, canExecute => _isGameRunning == false);
        public RelayCommand BoardSizeChangedCommand => new RelayCommand(execute => {
            if (execute is double newSize)
            {
                SetBoardSize((int)newSize);
            }
        }, canExecute => _isGameRunning == false);

        public MainWindowViewModel() 
        {
            UISquares = new ObservableCollection<SquareViewModel>();
		}

        private Color _defaultColor = Colors.AliceBlue;
        private Color _higlightColor  = Colors.Green;

        private bool _isGameRunning = false;
        private bool _isBusy = false;

        private SquareViewModel _firstSelected;
        private SquareViewModel _secondSelected;

        private List<Color> PlayerColors = new List<Color>();
		private int _playerAmount;
		public int SquareCount { get; set; }
		#region Data Binding


		// Vlastnosti, na nichž máme data binding: karty pexesa, velikost gridu (neměnné), skóre
		public ObservableCollection<SquareViewModel> UISquares { get; set; }
        public GameBoard Squares;
		public List<Player> TurnOrder = new List<Player>();
        public int MIN_PLAYERS { get; } = 2;
        public int MAX_PLAYERS { get; } = 5;

        public int MIN_FIELD_SIZE { get; } = 3;
        public int MAX_FIELD_SIZE { get; } = 9;
        private int _score;
        public int Score
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

        #endregion

        public void SetPlayerCount(int count)
        {
            PlayerAmount = count;
        }
        public void SetBoardSize(int size)
        {
            GridSize = size;
        }


        #region Herní logika
        // Samotná herní logika
        public HumanPlayer HumanPlayer;
		public void StartGame()
        {
            // TESTING
            UISquares.Clear();
            TurnOrder.Clear();
            CreateGameSquares();
            CreatePlayers();
            _isGameRunning = true;
            GameSequence();
		}
        private void CreateGameSquares()
        {
            if(GridSize == 0)
            {
                GridSize = MIN_FIELD_SIZE;
            }
			Squares = new GameBoard(GridSize);
			SquareCount = (int) Math.Pow(GridSize, 2);
            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    Position SquarePosition = new Position(x, y);
                    Square newSquare = new Square(SquarePosition);
                    SquareViewModel newViewModel = new SquareViewModel(newSquare, _defaultColor);
                    Squares.SetSquare(SquarePosition, newSquare);
                    UISquares.Add(newViewModel);
				}
            }
        }
        private void CreatePlayers()
        {
            if(PlayerAmount == 0)
            {
                PlayerAmount = MIN_PLAYERS;
            }
			Random random = new Random();
            List<Player> tempOrder = new List<Player>();
			PlayerColors.Add(Colors.Aqua);
            PlayerColors.Add(Colors.DarkRed);
            PlayerColors.Add(Colors.Green);
            PlayerColors.Add(Colors.Orange);
            PlayerColors.Add(Colors.Yellow);
            PlayerColors.Add(Colors.Olive);
            PlayerColors.Add(Colors.Magenta);
            PlayerColors.Add(Colors.Cyan);
            for (int i = 0; i < PlayerAmount; i++)
            {
                if (i == 0)
                {
                    HumanPlayer = new HumanPlayer(Colors.Blue);
					tempOrder.Add(HumanPlayer);
					continue;
                }
				Color randomColor = PlayerColors[i];
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
			}
			while (_isGameRunning)
            {
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
		}
        private async Task StartingPlayerTurn(Player currentPlayer)
        {
			bool madeValidMove = false;
			Position chosenPosition = new Position();
			while (!madeValidMove)
			{
				// čekání na tah hráče
				try
				{
					chosenPosition = await currentPlayer.TakeInitialTurn(Squares);
				}
				catch
				{
					break;
				}
				madeValidMove = WeakMoveValidityCheck(chosenPosition);
			}
			// zpracování tahu
			if (!madeValidMove)
			{
				return;
			}
			Square chosenSquare = Squares.GetSquare(chosenPosition);
			chosenSquare.changeOwner(currentPlayer);
			Score = HumanPlayer.Score;
		}

		private async Task PlayerTurn(Player currentPlayer)
		{
			bool madeValidMove = false;
			Position chosenPosition = new Position();
			while (!madeValidMove)
			{
				// čekání na tah hráče
				try
				{
					chosenPosition = await currentPlayer.TakeTurn(Squares);
				}
				catch
				{
                    currentPlayer.CanPlay = false;
					break;
				}
				madeValidMove = MoveValidityCheck(chosenPosition, currentPlayer);
			}
			// zpracování tahu
			if (!madeValidMove)
			{
                return;
			}
			Square chosenSquare = Squares.GetSquare(chosenPosition);
			chosenSquare.changeOwner(currentPlayer);
            // AUTOFILL
            List<Square> neighbours = Squares.GetNeighbours(chosenPosition);
            foreach (Square neighbour in neighbours)
            {
                if (neighbour.Owner is not null)
                {
                    continue;
                }
                bool result = Squares.IsUncontested(neighbour.Position, currentPlayer);
                if (!Squares.IsUncontested(neighbour.Position, currentPlayer))
                { 
                    continue;
                }
                // FLOOD FILL
                Squares.FloodFill(neighbour.Position, currentPlayer);
            }
			Score = HumanPlayer.Score;
		}
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

	}

}

    

