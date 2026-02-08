
namespace MVVMPexeso.Model.RTS_classes
{
	internal class RTSSquare : Square
	{
		public RTSPlayer Owner;
		public Position Position;
		public int Capacity;
		private RTSGameBoard GameBoard;

		public RTSSquare(Position position) : base(position)
		{
			Position = position;
			Capacity = 0;
		}
		public void ChangeOwner(RTSPlayer player)
		{
			if (player is not null)
			{
			}
		}
		public void UpdateCapacity()
		{
			List<RTSSquare> adjacent = GameBoard.GetNeighbours(Position);
			int numberOfAdjacentOwnedSquares = 0;
			foreach (RTSSquare adjacentSquare in adjacent)
			{
				if (adjacentSquare.Owner == Owner)
				{
					numberOfAdjacentOwnedSquares++;
				}
			}
			Capacity = CapacityTable.GetCapacity(numberOfAdjacentOwnedSquares);
		}
	}
}
