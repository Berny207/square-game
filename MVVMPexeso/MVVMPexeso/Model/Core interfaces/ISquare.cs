
using System.Windows.Media;

namespace MVVMPexeso.Model.Core_interfaces
{
	internal interface ISquare
	{
		internal Position GetPosition();
		internal IPlayer? GetOwner();
		internal void SetOwner(IPlayer newOwner);

		delegate void SquareUpdateHandler(ISquare square);

		internal void AddUpdateHandler(SquareUpdateHandler handler);

		internal void SetAsPossibleMove();

		internal void Clear();
		internal Color GetColor();
	}
}
