using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MVVMPexeso.Model.Enums
{
	internal class PlayerColors
	{
		public List<Color> Colors;
		public PlayerColors()
		{
			Colors = new List<Color>
			{
				System.Windows.Media.Colors.Aqua,
				System.Windows.Media.Colors.DarkRed,
				System.Windows.Media.Colors.Green,
				System.Windows.Media.Colors.Orange,
				System.Windows.Media.Colors.Yellow,
				System.Windows.Media.Colors.Olive,
				System.Windows.Media.Colors.Magenta,
				System.Windows.Media.Colors.Teal
			};
		}
		public Color PickRandomColor()
		{
			Random random = new Random();
			Color randomColor = Colors[random.Next(Colors.Count)];
			Colors.Remove(randomColor);
			return randomColor;
		}
	}
}
