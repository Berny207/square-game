using MVVMPexeso.Model.RTS_classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMPexeso.Model
{
	internal abstract class RTSPlayer : Player
	{
		public int Energy;
		public int Capacity;
		public List<RTSSquare> OwnedSquares = new List<RTSSquare>();
	}
}
