using MVVMPexeso.Model.Core_interfaces;
using MVVMPexeso.Model.TB_classes;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MVVMPexeso.Model
{
	internal class Square : ISquare
	{
		protected event ISquare.SquareUpdateHandler? SquareUpdated;

		private Color _color = Colors.LightGray;
		public IPlayer? Owner;
		private Position Position;
		public Square(Position position)
		{
			Position = position;
		}
		public void AddUpdateHandler(ISquare.SquareUpdateHandler handler)
		{
			SquareUpdated += handler;
		}

		public void SetOwner(IPlayer player)
		{
			if (player is not null)
			{
				Owner = player;
			}
			else
			{
				Owner = null;
			}
			
		}
		public Color GetColor()
		{
			return _color;
		}
		public Position GetPosition()
		{
			return Position;
		}
		public IPlayer? GetOwner()
		{
			return Owner;
		}
		public void Clear()
		{
			_color = Colors.LightGray;
			if (AddUpdateHandler != null)
			{
				SquareUpdated?.Invoke(this);
			}
		}
		public void SetColor(Color color)
		{
			_color = color;
			if(AddUpdateHandler != null)
			{
				SquareUpdated?.Invoke(this);
			}
		}
	}
}
