using MVVMPexeso.Model;
using MVVMProject.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MVVMPexeso.ViewModel
{
    internal class SquareViewModel : ViewModelBase
    {
        public Square Model { get; }
        public SquareViewModel(Square square)
        {
            Model = square;
        }

        // databindingované vlastnosti:

        private bool _isEmpty;
        public bool IsEmpty
        {
            get => _isEmpty;
            set { _isEmpty = value; OnPropertyChanged(); }
        }

        private SolidColorBrush _color;
        public SolidColorBrush Color
        {
            get => _color;
            set { _color = value; OnPropertyChanged(); }
        }
        public int Id => Model.Id;


    }
}
