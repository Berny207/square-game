using MVVMPexeso.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MVVMPexeso.Model
{
    internal class Square
    {
        public Player Owner;
        public Position Position;
        public SquareViewModel view;
        public Square(Position position)
        {
            Position = position;
        }
		public void changeOwner(Player player)
		{
			if (player is not null)
			{
				view.Brush = new SolidColorBrush(player.PlayerColor);
			}
			Owner = player;
            player.Score++;
			player.OwnedSquares.Add(this);
		}
	}
    internal struct Position
    {
        public int X;
        public int Y;
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
        public static Position operator +(Position a, Position b)
		{
			return new Position(a.X + b.X, a.Y + b.Y);
		}
	}
}
