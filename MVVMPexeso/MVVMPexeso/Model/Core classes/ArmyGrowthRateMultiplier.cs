using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMPexeso.Model.Enums
{
	internal class ArmyGrowthRateMultiplier
	{
		public double Multiplier { get; private set; }
		public int Duration { get; private set; }
		public string? Name { get; private set; }
		public ArmyGrowthRateMultiplier(double multiplier, int duration, string? name = null)
		{
			if (multiplier < 0)
			{
				throw new Exception("Multiplier cannot be negative.");
			}
			if (duration < 0)
			{
				throw new Exception("Duration cannot be negative.");
			}
			Multiplier = multiplier;
			Duration = duration;
			Name = name;
		}
		public void ReduceDuration()
		{
			Duration--;
		}
	}
}
