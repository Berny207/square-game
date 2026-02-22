using MVVMPexeso.Model.Core_classes;
using MVVMPexeso.Model.Core_interfaces;
using MVVMPexeso.Model.Enums;
using MVVMPexeso.Model.RTS_classes;
using MVVMPexeso.Model.TB_classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static System.Formats.Asn1.AsnWriter;

namespace MVVMPexeso.Model
{
	internal class RTSAIPlayer : RTSPlayer
	{
		AIActions action;
		private readonly double ARMY_AT_HOME = 0.7;
		private readonly double CONNECTION_BENEFIT = 0.5;
		private readonly double EXPANSION_AGGRESION = 0.5;
		private readonly double MINIMAL_ATTACK_ADVANTAGE = 0.2;
		private double minimalTimeBetweenActions = 750; // in milliseconds
		private Position? lastTakenPosition;
		private RTSPlayer weakestNeighbour;
		private Stopwatch timeSinceLastAction = new Stopwatch();
		public void setLastTakenPosition(Position position)
		{
			this.lastTakenPosition = position;
		}
		public RTSAIPlayer(Color playerColor, RTSGameManager gameManager)
		{
			this.SetColor(playerColor);
			this.SetGameManager(gameManager);
			timeSinceLastAction.Start();
		}
		public ISquare? CalculateTurn(RTSGameManager gameManager)
		{
			if(timeSinceLastAction.ElapsedMilliseconds < minimalTimeBetweenActions)
			{
				return null; // Not enough time has passed since last action, wait
			}
			timeSinceLastAction.Restart();
			this.action = DecideAction(gameManager);
			Console.WriteLine($"AI decided to {action}");
			switch (action)
			{
				case AIActions.Attack:
					return DecideAttack(this.GetPossibleMoves(), gameManager, weakestNeighbour);
				case AIActions.Defend:
					return DecideDefend(this.GetPossibleMoves(), gameManager);
				case AIActions.Expand:
					return DecideExpansion(this.GetPossibleMoves(), gameManager);
				case AIActions.Hold:
					return null;
				default:
					throw new Exception("Invalid AI action");
			}

		}
		private AIActions DecideAction(RTSGameManager gameManager)
		{
			// Is expanding possible?
			// First of all, is my army big enough to expand?

			if (gameManager.CAPTURE_COST_EMPTY > this.GetArmy() * (1 - ARMY_AT_HOME))
			{
				// Low army, decide to defend or hold
				if (gameManager.CAPTURE_COST_EMPTY < this.GetArmy() && lastTakenPosition is not null)
				{
					return AIActions.Defend; // Too low army, decide to hold
				}
				else
				{
					return AIActions.Hold; // Can defend -> defend at all costs
				}
			}
			//Decide of expansion
			List<ISquare> emptyPossibleMoves = new List<ISquare>();
			foreach (ISquare possibleMove in this.GetPossibleMoves())
			{
				if (possibleMove.GetOwner() is null)
				{
					emptyPossibleMoves.Add(possibleMove);
				}
			}
			if (emptyPossibleMoves.Count > 0)
			{
				return AIActions.Expand;
			}
			//Decide of attack
			//we need to put here decision of when to attack
			// first, get our neighbouring players and see, if they have low army
			List<RTSPlayer> neighbouringPlayers = new List<RTSPlayer>();
			foreach(ISquare possibleMove in this.GetPossibleMoves())
			{
				if (possibleMove.GetOwner() != null && possibleMove.GetOwner() != this)
				{
					RTSPlayer player = (RTSPlayer)possibleMove.GetOwner();
					if (!neighbouringPlayers.Contains(player))
					{
						neighbouringPlayers.Add(player);
					}
				}
			}
			weakestNeighbour = null;
			double minimalArmy = int.MaxValue;
			foreach (RTSPlayer player in neighbouringPlayers)
			{
				if (player.GetArmy() * MINIMAL_ATTACK_ADVANTAGE < this.GetArmy())
				{
					if (player.GetArmy() < minimalArmy)
					{
						minimalArmy = player.GetArmy();
						weakestNeighbour = player;
					}
				}
			}
			if(weakestNeighbour != null)
			{
				return AIActions.Attack;
			}

			//There's nothing we can do, hold
			return AIActions.Hold;
		}
		private ISquare DecideExpansion(List<ISquare> possibleMoves, RTSGameManager gameManager)
		{
			List<ISquare> emptyPossibleMoves = new List<ISquare>();
			foreach (ISquare possibleMove in possibleMoves)
			{
				if (possibleMove.GetOwner() is null)
				{
					emptyPossibleMoves.Add(possibleMove);
				}
			}
			if (emptyPossibleMoves.Count == 0)
			{
				throw new Exception("No empty possible moves for expansion");
			}
			double bestBenefit = double.NegativeInfinity;
			ISquare? bestExpansionMove = new Square(new Position(0, 0));
			foreach (ISquare emptyMove in emptyPossibleMoves)
			{
				double benefit = 0;
				foreach (ISquare neighbouringSquare in gameManager.GetGameBoard().GetNeighbours(emptyMove.GetPosition()))
				{
					if (neighbouringSquare.GetOwner() == this)
					{
						benefit += CONNECTION_BENEFIT;
						continue;
					}
					if (neighbouringSquare.GetOwner() != null)
					{
						benefit -= 1 - EXPANSION_AGGRESION;
						continue;
					}
				}
				if (benefit > bestBenefit)
				{
					bestBenefit = benefit;
					bestExpansionMove = emptyMove;
				}
			}
			return bestExpansionMove;
		}
		private ISquare DecideDefend(List<ISquare> possibleMoves, RTSGameManager gameManager)
		{
			if (lastTakenPosition is null) // need to have recently taken square
			{
				throw new Exception("No last taken position for defend action");
			}
			Position pos = (Position)lastTakenPosition;
			lastTakenPosition = null; // reset last taken position, so it won't affect next turns
			return gameManager.GetGameBoard().GetSquare(pos);
		}
		private ISquare DecideAttack(List<ISquare> possibleMoves, RTSGameManager gameManager, RTSPlayer playerToAttack)
		{
			int bestOwnBenefit = int.MinValue;
			int bestEnemyLoss = int.MinValue;
			ISquare? bestMove = null;
			foreach (ISquare possibleMove in possibleMoves)
			{
				if (possibleMove.GetOwner() == playerToAttack)
				{
					int ownBenefit = 0;
					int enemyLoss = 0;
					List<ISquare> neighbours = gameManager.GetGameBoard().GetNeighbours(possibleMove.GetPosition());
					foreach (ISquare neighbour in neighbours)
					{
						if (neighbour.GetOwner() == this)
						{
							bestOwnBenefit++;
						}
						else if (neighbour.GetOwner() == playerToAttack)
						{
							bestEnemyLoss++;
						}
					}
					if (ownBenefit > bestOwnBenefit || (ownBenefit == bestOwnBenefit && enemyLoss > bestEnemyLoss))
					{
						bestMove = possibleMove;
						bestOwnBenefit = ownBenefit;
						bestEnemyLoss = enemyLoss;
					}
				}
			}
			if(bestMove == null)
			{
				throw new Exception("No valid attack move found");
			}
			return bestMove;
		}
	}
}
