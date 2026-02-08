using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMPexeso.Model.Enums
{
	internal static class Directions
	{
		internal static IReadOnlyList<Position> DirectionsBase = new List<Position>
		{
			new Position(0, 1),  // Up
			new Position(1, 0),  // Right
			new Position(0, -1), // Down
			new Position(-1, 0)  // Left
		};
	}
}
