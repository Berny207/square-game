using MVVMPexeso.Model.Core_classes;
using MVVMPexeso.Model.Core_interfaces;
using MVVMPexeso.Model.Enums;
using MVVMPexeso.Model.TB_classes;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MVVMPexeso.Model.RTS_classes
{
	internal class RTSGameManager : GameManager
	{
		// private const double ARMY_PER_CONNECTION = 0.5;
		public readonly double ARMY_PER_SQUARE = 0.1;
		public readonly int CAPACITY_PER_SQUARE = 3;
		public readonly int CAPACITY_PER_CONNECTION = 2;
		public readonly int CAPACITY_PER_HOME_SQUARE = 15;
		public readonly int TICKS_PER_SECOND = 1;
		public readonly int CAPTURE_COST_EMPTY = 1;
		public readonly int CAPTURE_COST_ENEMY = 2;

		public readonly double STARTING_MULTIPLIER = 10;
		private RTSHumanPlayer? humanPlayer;
		private Stopwatch clock = new Stopwatch();
		protected RTSGameBoard RTSBoard
		{
			get
			{
				if (Board is null)
				{
					throw new Exception("Tried using null Board value.");
				}
				if ((RTSGameBoard)Board is null)
				{
					throw new Exception("Tried parsing IGameBoard as TBGameBoard");
				}
				return (RTSGameBoard)Board;
			}
		}
		public RTSGameManager(int gridSize, int playerCount)
		{
			TurnOrder = new List<IPlayer>();
			PreparePlayers(playerCount);
			humanPlayer = TurnOrder[0] as RTSHumanPlayer;
			Board = new RTSGameBoard(gridSize);
		}
		public override void Game()
		{
			IsGameRunning = true;
			if (Board is null)
			{
				return;
			}
			foreach (RTSPlayer player in TurnOrder)
			{
				bool result = SelectStartPosition(player);
				if (result == false)
				{
					EndGame("Game creation failed.");
					return;
				}
				if (player is RTSHumanPlayer)
					UpdateUI(player);
			}
			clock = Stopwatch.StartNew();
			while (GetGameRunning())
			{
				UpdateArmies();
				CheckInputRequest();
				if (humanPlayer is not null) 
				{ 
					UpdateUI(humanPlayer);
				}
				AITurns();
				UpdatePlayers();
			}
			clock.Stop();
		}
		#region game prep
		private void PreparePlayers(int playerCount)
		{
			PlayerColors playerColours = new PlayerColors();
			for (int i = 0; i < playerCount; i++)
			{
				if (i == 0)
				{
					TurnOrder.Add(new RTSHumanPlayer(Colors.Blue));
				}
				else
				{
					TurnOrder.Add(new RTSAIPlayer(playerColours.PickRandomColor(), this));
				}
				RTSPlayer player = (RTSPlayer)TurnOrder[i];
				player.AddArmyGrowthMultiplier(new ArmyGrowthRateMultiplier(STARTING_MULTIPLIER, 5, "Starting multiplier"));
			}
		}
		bool SelectStartPosition(RTSPlayer player)
		{
			RTSSquare? square = null;
			try
			{
				square = (RTSSquare)RTSBoard.GetRandomEmptySquare();
			}
			catch
			{
				return false;
			}
			square.ChangeHome(true);
			SetSquareOwner(square, player);
			return true;
		}
		#endregion
		#region game loop
		private void UpdateArmies()
		{
			if(clock.ElapsedMilliseconds < 1000 / TICKS_PER_SECOND)
			{
				return;
			}
			clock.Restart();
			foreach(RTSPlayer player in TurnOrder)
			{
				player.ChangeArmy(player.GetOwnedSquares().Count*ARMY_PER_SQUARE);
			}
		}
		void CheckInputRequest()
		{
			var square = PopUserInput();
			if (square is not null)
			{
				SquareClicked(square);
			}
		}
		private void AITurns()
		{
			foreach(RTSPlayer player in TurnOrder)
			{
				if(player is RTSAIPlayer)
				{
					RTSAIPlayer AIPlayer = (RTSAIPlayer)player;
					ISquare? move = AIPlayer.CalculateTurn(this);
					if(move is not null)
					{
						DoPlayerTurn(AIPlayer, move);
						UpdateUI(humanPlayer);
					}
				}
			}
		}
		private void UpdatePlayers()
		{
			for(int playerID = TurnOrder.Count-1;playerID>=0;playerID--)
			{
				IPlayer player = TurnOrder[playerID];
				if (player.GetOwnedSquares().Count == 0)
				{
					foreach (ISquare somehowPossibleMove in player.GetPossibleMoves())
					{
						Console.WriteLine(somehowPossibleMove.GetPosition());
					}
					if (player is RTSHumanPlayer)
					{
						EndGame("You have been defeated");
					}
					TurnOrder.Remove(player);
				}
			}
			if(TurnOrder.Count == 1 && TurnOrder[0] is RTSHumanPlayer)
			{
				EndGame("You are victorious!");
			}
		}
		#endregion
		private void SetSquareOwner(RTSSquare square, RTSPlayer player)
		{
			RTSPlayer? originalOwner = (RTSPlayer?)square.GetOwner();
			if(originalOwner is RTSAIPlayer)
			{
				((RTSAIPlayer)originalOwner).setLastTakenPosition(square.GetPosition());
			}
			square.SetOwner(player);
			square.SetColor(player.GetColor());
			player.AddSquare(square);
			int connectionCount = 0;
			// Calculate the amount of interconnected squares
			List<ISquare> neighbours = Board.GetNeighbours(square.GetPosition());
			foreach (ISquare neighbour in neighbours)
			{
				if (neighbour.GetOwner() == player)
				{
					connectionCount++;
				}
			}
			// Destroy captured homes so attacker doesn't benefit
			// Note - lost homes cannot be rebuild. The change is permanent
			if(originalOwner is not null && square.IsHome())
			{
				square.ChangeHome(false);
			}
			// Calculate square's new capacity and update player capacity
			int squareCapacity = 0;
			if (square.IsHome())
			{
				squareCapacity = CAPACITY_PER_HOME_SQUARE;
			}
			else
			{
				squareCapacity = CAPACITY_PER_SQUARE;
			}
			player.ChangeCapacity(CAPACITY_PER_CONNECTION * connectionCount + squareCapacity);

			// If there was an original owner, we need to update their owned squares and capacity as well.
			if (originalOwner is not null)
			{
				originalOwner.RemoveSquare(square);
				foreach(ISquare neighbour in neighbours)
				{
					if (neighbour.GetOwner() == originalOwner)
					{
						originalOwner.ChangeCapacity(-CAPACITY_PER_CONNECTION);
						break;
					}
				}
				originalOwner.ChangeCapacity(-CAPACITY_PER_SQUARE);
			}
			// Don't forget to update possible moves for both players.
			UpdatePlayerPossibleMoves(player, square, originalOwner);

			// Update UI if human player is involved.
			if (player is RTSHumanPlayer || originalOwner is RTSHumanPlayer)
				UpdateUI(player);
		}
		private bool DoPlayerTurn(RTSPlayer player, ISquare square)
		{
			// is turn correct?
			if (!player.IsSquarePossibleMove(square))
			{
				return false;
			}

			if(square.GetOwner() != null && player.GetArmy() < CAPTURE_COST_ENEMY)
			{
				return false;
			}
			if(square.GetOwner() == null && player.GetArmy() < CAPTURE_COST_EMPTY)
			{
				return false;
			}
			SetSquareOwner((RTSSquare)square, (RTSPlayer)player);
			return true;
		}
		public override void SquareClicked(ISquare square)
		{
			if(humanPlayer is null)
			{
				return;
			}
			int moveCost = 0;
			if(square.GetOwner() is not null)
			{
				moveCost = CAPTURE_COST_ENEMY;
			}
			else
			{
				moveCost = CAPTURE_COST_EMPTY;
			}
			if (humanPlayer.GetArmy() < moveCost)
			{
				return;
			}
			bool turnResult = false;
			turnResult = DoPlayerTurn(humanPlayer, square);
			if (!turnResult)
			{
				return;
			}
			humanPlayer.ChangeArmy(-moveCost);
		}
		private void UpdatePlayerPossibleMoves(IPlayer player, ISquare square, IPlayer? originalOwner)
		{
			player.RemovePossibleMove(square);
			foreach(ISquare neighbour in Board.GetNeighbours(square.GetPosition()))
			{
				if (!player.IsSquarePossibleMove(neighbour) && neighbour.GetOwner() != player)
				{
					player.AddPossibleMove(neighbour);
				}
				if (originalOwner is null)
				{
					continue;
				}
				if(neighbour.GetOwner() == originalOwner)
				{
					if (!originalOwner.IsSquarePossibleMove(neighbour))
					{
						originalOwner.AddPossibleMove(neighbour);
					}
					continue;
				}
				bool originalOwnerHasAccess = false;
				// We need to check the neighbour neighbours if the original owner has still access to this square.
				foreach(ISquare secondNeighbour in Board.GetNeighbours(neighbour.GetPosition()))
				{
					if(secondNeighbour.GetOwner() == originalOwner)
					{
						originalOwnerHasAccess = true;
						break;
					}
				}
				if (!originalOwnerHasAccess)
				{
					originalOwner.RemovePossibleMove(neighbour);
				}
			}
		}
	}
}
