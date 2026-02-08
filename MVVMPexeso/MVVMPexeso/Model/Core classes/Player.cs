using MVVMPexeso.Model.Core_interfaces;
using System.Windows.Media;

namespace MVVMPexeso.Model
{
    internal abstract class Player : IPlayer
    {
        private int Score;
        private List<ISquare> PossibleMoves = new List<ISquare>();
        private List<ISquare> OwnedSquares = new List<ISquare>();
        private Color PlayerColor;
		private IGameManager gameManager;
		public int GetScore()
        {
            return Score;
        }
        public void SetScore(int score)
        {
            Score = score;
        }
        public List<ISquare> GetOwnedSquares()
        {
            return OwnedSquares;
        }
        public void AddSquare(ISquare square)
        {
            OwnedSquares.Add(square);
        }
        public void AddPossibleMove(ISquare square)
        {
            if (this is TBHumanPlayer)
            {
                square.SetAsPossibleMove();
            }
			PossibleMoves.Add(square);
        }
        public void RemovePossibleMove(ISquare square)
        {
			PossibleMoves.Remove(square);
        }
        public Color GetColor()
        {
            return PlayerColor;
        }
        public bool IsMoveLegal(ISquare square)
        {
            return PossibleMoves.Contains(square);
		}
        public void SetColor(Color color)
        {
            PlayerColor = color;
		}
        public List<ISquare> GetPossibleMoves()
        {
            return PossibleMoves;
		}
        public IGameManager GetGameManager()
        {
            return gameManager;
        }
        public void SetGameManager(IGameManager gameManager)
        {
            this.gameManager = gameManager;
        }
	}
}
