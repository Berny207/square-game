using MVVMPexeso.Model;
using MVVMPexeso.Model.Core_interfaces;
using MVVMProject.MVVM;
using System.Windows;
using System.Windows.Media;

namespace MVVMPexeso.ViewModel
{
    internal class SquareViewModel : ViewModelBase
    {
        public ISquare Model { get; }
        public SquareViewModel(ISquare square, Color color)
        {
            Model = square;
            Brush = new SolidColorBrush(color);
            square.AddUpdateHandler(OnSquareUpdated);
		}
		public SquareViewModel() { }

		void OnSquareUpdated(ISquare square)
		{
			var brush = new SolidColorBrush(square.GetColor());
			brush.Freeze();
			Brush = brush;
		}
		// databindingované vlastnosti:
		private SolidColorBrush _brush;
        public SolidColorBrush Brush
        {
            get => _brush;
            set 
			{
				_brush = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HoverColor));
			}
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
