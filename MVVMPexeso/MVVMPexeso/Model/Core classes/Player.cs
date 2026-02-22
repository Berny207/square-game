using MVVMPexeso.Model.Core_interfaces;
using System.Windows.Media;

namespace MVVMPexeso.Model
{
    internal abstract class Player : IPlayer
    {
        private Dictionary<String, ISquare> PossibleMoves = new Dictionary<String, ISquare>();
        private List<ISquare> OwnedSquares = new List<ISquare>();
        private Color PlayerColor;
		private IGameManager? gameManager;
        public List<ISquare> GetOwnedSquares()
        {
            return OwnedSquares;
        }
        public void AddSquare(ISquare square)
        {
            OwnedSquares.Add(square);
        }
        public void RemoveSquare(ISquare square)
        {
            OwnedSquares.Remove(square);
        }
        public void AddPossibleMove(ISquare square)
        {
            if (this is TBHumanPlayer)
            {
                square.SetColor(Colors.DarkGray);
            }
            PossibleMoves.Add(square.GetPosition().ToString(), square);
        }
        public void RemovePossibleMove(ISquare square)
        {
			PossibleMoves.Remove(square.GetPosition().ToString());
        }
        public Color GetColor()
        {
            return PlayerColor;
        }
        public bool IsSquarePossibleMove(ISquare square)
        {
            return PossibleMoves.ContainsKey(square.GetPosition().ToString());
		}
        public void SetColor(Color color)
        {
            PlayerColor = color;
		}
        public List<ISquare> GetPossibleMoves()
        {
            return PossibleMoves.Values.ToList<ISquare>();
		}
        public IGameManager GetGameManager()
        {
			if (gameManager is null)
			{
				throw new Exception("Tried returning null gameManager.");
			}
			return gameManager;
        }
        public void SetGameManager(IGameManager gameManager)
        {
            this.gameManager = gameManager;
        }
	}
}
