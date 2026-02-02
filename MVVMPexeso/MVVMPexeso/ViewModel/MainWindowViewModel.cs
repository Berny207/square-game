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

        private const int MIN_PLAYERS = 1;
        private const int MAX_PLAYERS = 4;

        private const int MIN_FIELD_SIZE = 3;
        private const int MAX_FIELD_SIZE = 9;

        private List<Color> PlayerColors = new List<Color>();
		private int _playerAmount;
		public int SquareCount { get; set; }
		#region Data Binding


		// Vlastnosti, na nichž máme data binding: karty pexesa, velikost gridu (neměnné), skóre
		public ObservableCollection<SquareViewModel> UISquares { get; set; }
        public GameBoard Squares;
		public List<Player> TurnOrder = new List<Player>();
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
				if (MIN_PLAYERS < value & value <= MAX_PLAYERS)
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

        #region Herní logika
        // Samotná herní logika
        public HumanPlayer HumanPlayer;
		public void StartGame()
        {
			// TESTING
			GridSize = 8;
			PlayerAmount = 4;
			CreateGameSquares();
            CreatePlayers();
            _isGameRunning = true;
            GameSequence();
		}
        private void CreateGameSquares()
        {
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
				Color randomColor = PlayerColors[random.Next(PlayerColors.Count)];
				PlayerColors.Remove(randomColor);
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
                foreach (Player currentPlayer in TurnOrder)
                {
                    await PlayerTurn(currentPlayer);
					if (!_isGameRunning)
					{
						// zpracuj konec hry
						return;
					}
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
			Console.WriteLine("Square clicked at position: " + clicked.Model.Position.X + ", " + clicked.Model.Position.Y);
			HumanPlayer.SquareClicked(clicked.Model.Position);
		}
        #endregion

	}

}

    

