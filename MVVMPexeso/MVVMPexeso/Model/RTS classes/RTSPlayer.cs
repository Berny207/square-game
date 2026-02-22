using MVVMPexeso.Model.Core_interfaces;
using MVVMPexeso.Model.Enums;
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
		private double Army;
		private int Capacity;
		private List<ArmyGrowthRateMultiplier> ArmyGrowthMultipliers = new List<ArmyGrowthRateMultiplier>();
		public double GetArmy()
		{
			return Army;
		}
		public int GetCapacity()
		{
			return Capacity;
		}
		public void ChangeArmy(double amount)
		{
			double finalMultiplier = 1;
			for(int i = ArmyGrowthMultipliers.Count - 1; i >= 0; i--)
			{
				ArmyGrowthRateMultiplier multipler = ArmyGrowthMultipliers[i];
				multipler.ReduceDuration();
				if(this is RTSHumanPlayer)
				{
				}
				if(multipler.Duration < 0)
				{
					ArmyGrowthMultipliers.RemoveAt(i);
					continue;
				}
				finalMultiplier *= ArmyGrowthMultipliers[i].Multiplier;
			}
			if (amount > 0)
			{
				amount *= finalMultiplier;
			}
			Army += amount;
			if (Army > Capacity)
			{
				Army = Capacity;
			}
			if(Army < 0)
			{
				throw new Exception("Army cannot be negative.");
			}
		}
		public void ChangeCapacity(int amount)
		{
			Capacity += amount;
			if (Capacity < 0)
			{
				throw new Exception("Capacity cannot be negative.");
			}
			if(Army > Capacity)
			{
				Army = Capacity;
			}
		}
		public void AddArmyGrowthMultiplier(ArmyGrowthRateMultiplier multiplier)
		{
			ArmyGrowthMultipliers.Add(multiplier);
		}
		public void RemoveArmyGrowthMultiplier(ArmyGrowthRateMultiplier multiplier)
		{
			ArmyGrowthMultipliers.Remove(multiplier);
		}
	}
}
