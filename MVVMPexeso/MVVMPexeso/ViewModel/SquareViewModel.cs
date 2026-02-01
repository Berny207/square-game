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
        public SquareViewModel(Square square, Color color)
        {
            Model = square;
            Brush = new SolidColorBrush(color);
            square.view = this;
		}

        // databindingované vlastnosti:
        private SolidColorBrush _brush;
        public SolidColorBrush Brush
        {
            get => _brush;
            set { _brush = value; OnPropertyChanged(); OnPropertyChanged(nameof(HoverColor)); }
        }
		private Color Lighten(Color color, double factor)
		{
			if (factor < 0) factor = 0;
			if (factor > 1) factor = 1;

			byte lighten(byte channel) => (byte)(channel + (255 - channel) * factor);

			return Color.FromRgb(
				lighten(color.R),
				lighten(color.G),
				lighten(color.B)
			);
		}
		public SolidColorBrush HoverColor
		{
			get => new SolidColorBrush(Lighten(Brush.Color, 0.3));
		}
	}
}
