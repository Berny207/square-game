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
        public RelayCommand CardClickCommand => new RelayCommand(execute => SquareClicked(execute as SquareViewModel), canExecute => _isGameRunning == true);
        public RelayCommand PlayerCountChangedCommand => new RelayCommand(execute => {
            if (execute is double newSize)
            {
                SetPlayerCount((int)newSize);
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
            Squares = new ObservableCollection<SquareViewModel>();
        }

        private Color _defaultColor = Colors.AliceBlue;
        private Color _higlightColor  = Colors.Green;

        private bool _isGameRunning = false;
        private bool _isBusy = false;

        private SquareViewModel _firstSelected;
        private SquareViewModel _secondSelected;


        int BoardSize = 5;
        int PlayerCount = 3;


        #region Data Binding

        // Vlastnosti, na nichž máme data binding: karty pexesa, velikost gridu (neměnné), skóre
        public ObservableCollection<SquareViewModel> Squares { get; set; }

        public int GridSize => (int)Math.Sqrt(Squares.Count);

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

        #endregion

        public void SetPlayerCount(int count)
        {
            PlayerCount = count;
        }
        public void SetBoardSize(int size)
        {
            BoardSize = size;
        }


        #region Herní logika
        // Samotná herní logika
        public void StartGame()
        {   
            CreateGameCards();
            OnPropertyChanged(nameof(GridSize)); // máme nachystané karty, vyvoláme funkci, že se grid změnil
            _isGameRunning = true;
        }
        private void CreateGameCards()
        {
            // přidáme čtverce
            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    Squares.Add(new SquareViewModel(new Square(new Vector2(x, y))));
                }
            }
        }


        private async void SquareClicked(SquareViewModel clicked)
        {
            if (!clicked.IsEmpty)
            {
               
            }

            if (Score == 2)
                _isGameRunning = false;

            
        }


        #endregion

    }

}

    

