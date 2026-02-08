using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace MVVMPexeso.Model.RTS_classes
{
	internal static class CapacityTable
	{
		public static IReadOnlyDictionary<int, int> Table { get; }
		static CapacityTable()
		{
			Table = new Dictionary<int, int>
			{
				{ 0, 1 },
				{ 1, 2 },
				{ 2, 3 },
				{ 3, 5 },
				{ 4, 8 }
			};
		}
		public static int GetCapacity(int numberOfAdjacentOwnedSquares)
		{
			if (Table.ContainsKey(numberOfAdjacentOwnedSquares))
			{
				return Table[numberOfAdjacentOwnedSquares];
			}
			else
			{
				throw new ArgumentOutOfRangeException(nameof(numberOfAdjacentOwnedSquares), "Number of adjacent owned squares must be between 0 and 4.");
			}
		}
	}
}
