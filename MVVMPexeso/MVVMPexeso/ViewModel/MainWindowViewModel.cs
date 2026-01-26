using MVVMPexeso.Model;
using MVVMProject.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
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
        public RelayCommand CardClickCommand => new RelayCommand(execute => CardClicked(execute as SquareViewModel), canExecute => _isGameRunning == true);

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


        const int CardCount = 16;

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

        #region Herní logika
        // Samotná herní logika
        public void StartGame()
        {   
            CreateGameCards();
            ShuffleCards();
            OnPropertyChanged(nameof(GridSize)); // máme nachystané karty, vyvoláme funkci, že se grid změnil
            _isGameRunning = true;
        }
        private void CreateGameCards()
        {
            // přidáme dvojice karet
            for (int i = 0; i < CardCount / 2; i++)
            {
                Squares.Add(new SquareViewModel(new Square(i)));
                Squares.Add(new SquareViewModel(new Square(i)));
            }
        }

        private void ShuffleCards()
        {
            Random rng = new Random();
            int n = Squares.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (Squares[i], Squares[j]) = (Squares[j], Squares[i]); // Swap
            }
        }

        private async void CardClicked(SquareViewModel clicked)
        {
            if (_isBusy) return; // probíhá čekání u 2 ukázaných karet

            if (clicked.IsEmpty || clicked.IsMatched) return;

            
            clicked.IsEmpty = true;

            if (_firstSelected == null)
            {
                _firstSelected = clicked;
                return;
            }

            _secondSelected = clicked;
            _isBusy = true;  // začínáme čekat

            // čekáme 1 sekundu, aby hráč viděl druhou kartu
            await Task.Delay(1000);
            

            // porovnáme, zda se karty shodují
            if (_firstSelected.Model.Id == _secondSelected.Model.Id)
            {
                _firstSelected.IsMatched = true;
                _secondSelected.IsMatched = true;
                Score++;
            }
            else // pokud ne, otočíme je zpátky
            {
                _firstSelected.IsEmpty = false;
                _secondSelected.IsEmpty = false;
            }

            _firstSelected = null;
            _secondSelected = null;

            _isBusy = false; // konec čekání

            if (Score == CardCount / 2)
                _isGameRunning = false;

            
        }


        #endregion

    }

}

    

