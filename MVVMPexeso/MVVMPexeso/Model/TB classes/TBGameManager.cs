using MVVMPexeso.Model.Core_classes;
using MVVMPexeso.Model.Core_interfaces;
using MVVMPexeso.Model.Enums;
using System.Windows.Media;

namespace MVVMPexeso.Model.TB_classes
{
	internal class TBGameManager : GameManager
	{
		TBPlayer currentPlayer;

		protected TBGameBoard TBBoard { get { return (TBGameBoard)Board; } }

		public TBGameManager(int gridSize, int playerCount)
		{
			TurnOrder = new List<IPlayer>();
			PreparePlayers(playerCount);
			Board = new TBGameBoard(gridSize);
		}
		public override void Game()
		{
			IsGameRunning = true;
			currentPlayer = (TBPlayer)TurnOrder[0];
			foreach(TBPlayer player in TurnOrder)
			{
				SelectStartPosition(player);
				if(player is TBHumanPlayer)
					UpdateScore(player);
			}
			int skipCounter = 0;
			while (GetGameRunning())
			{
				if(skipCounter >= TurnOrder.Count || !IsGameRunning)
				{
					if (!IsGameRunning)
						Board.ClearBoard();
					return;
				}
				if (currentPlayer.GetPossibleMoves().Count == 0)
				{
					NextPlayer();
					skipCounter++;
					continue;
				}
				skipCounter = 0;
				if (currentPlayer is TBAIPlayer)
				{
					TBAIPlayer AIPlayer = currentPlayer as TBAIPlayer;
					TBSquare move = AIPlayer.CalculateTurn(this);
					DoPlayerTurn(AIPlayer, move);
					NextPlayer();
				}
				else
				{
					CheckInputRequest();
				}
			}
		}
		private void NextPlayer()
		{
			int index = TurnOrder.IndexOf(currentPlayer);
			currentPlayer = (TBPlayer)TurnOrder[(index + 1) % TurnOrder.Count];
		}
		void CheckInputRequest()
		{
			var square = PopUserInput();
			if (square is not null)
			{
				SquareClicked(square);
			}
		}
		void SelectStartPosition(TBPlayer player)
		{
			TBSquare square = TBBoard.GetRandomEmptySquare();
			square.SetOwner(player);
			UpdatePlayerPossibleMoves(player, square);
		}
		private void PreparePlayers(int playerCount)
		{
			PlayerColors playerColours = new PlayerColors();
			for (int i = 0; i < playerCount; i++)
			{
				if(i == 0)
				{
					TurnOrder.Add(new TBHumanPlayer(Colors.Blue));
					continue;
				}
				TurnOrder.Add(new TBAIPlayer(playerColours.PickRandomColor(), this));
			}
		}
		public override void SquareClicked(ISquare squareP)
		{
			TBSquare square = squareP as TBSquare;
			if(currentPlayer is not TBHumanPlayer)
			{
				return;
			}
			// process player's input
			bool turnResult = false;
			turnResult = DoPlayerTurn(currentPlayer, square);
			if(!turnResult)
			{
				return;
			}
			UpdateScore(currentPlayer);
			NextPlayer();
		}
		private bool DoPlayerTurn(TBPlayer player, TBSquare square)
		{
			// is turn correct?
			if(!player.IsMoveLegal(square))
			{
				return false;
			}
			square.SetOwner(player);
			DoAutoFill(player, square);
			UpdatePlayerPossibleMoves(player, square);
			return true;
		}
		private void UpdatePlayerPossibleMoves(TBPlayer player, TBSquare square)
		{
			player.RemovePossibleMove(square);
			List<ISquare> neighbours = TBBoard.GetNeighbours(square.GetPosition());
			foreach(TBSquare neighbour in neighbours)
			{
				TBPlayer? neighbourOwner = neighbour.GetOwner() as TBPlayer;
				if(neighbourOwner is null)
				{
					player.AddPossibleMove(neighbour);
				}
				else if(neighbourOwner != player)
				{
					neighbourOwner.RemovePossibleMove(square);
				}
			}
		}
		private void DoAutoFill(TBPlayer player, TBSquare square)
		{
			List<ISquare> neighbours = Board.GetNeighbours(square.GetPosition());
			foreach(TBSquare neighbour in neighbours)
			{
				if(neighbour.Owner is not null)
				{
					continue;
				}
				bool uncontested = TBBoard.IsUncontested(neighbour, player);
				if (!uncontested)
				{
					continue;
				}
				TBBoard.FloodFill(neighbour, player);
			}
		}
	}
}
