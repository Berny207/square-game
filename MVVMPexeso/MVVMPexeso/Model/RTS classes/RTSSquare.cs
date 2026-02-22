
using MVVMPexeso.Model.Core_interfaces;

namespace MVVMPexeso.Model.RTS_classes
{
	internal class RTSSquare : Square
	{
		private bool HomeSquare;
		public RTSSquare(Position position) : base(position)
		{
			HomeSquare = false;
		}
		public bool IsHome()
		{
			return HomeSquare;
		}
		public void ChangeHome(bool value)
		{
			HomeSquare = value;
		}
	}
}
