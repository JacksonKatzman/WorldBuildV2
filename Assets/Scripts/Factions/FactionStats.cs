using System.Collections;
using UnityEngine;

namespace Game.Factions
{
	[System.Serializable]
	public class FactionStats : ITimeSensitive
	{
		public ModifiedInt actionsPerTurn;

		public ModifiedFloat birthRate;
		public ModifiedFloat deathRate;
		public ModifiedFloat foodProductionPerWorker;
		public ModifiedFloat spoilageRate;
		public ModifiedFloat maxFoodByLand;
		public ModifiedFloat maxBurgeoningTension;
		public ModifiedFloat rebellionChance;
		public ModifiedFloat factionPressureModifier;
		public ModifiedFloat factionPressureThreshold;

		public FactionStats()
		{
			actionsPerTurn = new ModifiedInt(0);

			birthRate = new ModifiedFloat(0);
			deathRate = new ModifiedFloat(0);
			foodProductionPerWorker = new ModifiedFloat(0);
			spoilageRate = new ModifiedFloat(0);
			maxFoodByLand = new ModifiedFloat(0);
			maxBurgeoningTension = new ModifiedFloat(0);
			rebellionChance = new ModifiedFloat(0);
			factionPressureModifier = new ModifiedFloat(1.0f);
			factionPressureThreshold = new ModifiedFloat(0);
		}

		public void AdvanceTime()
		{
			actionsPerTurn.AdvanceTime();

			birthRate.AdvanceTime();
			deathRate.AdvanceTime();
			foodProductionPerWorker.AdvanceTime();
			spoilageRate.AdvanceTime();
			maxFoodByLand.AdvanceTime();
			maxBurgeoningTension.AdvanceTime();
			rebellionChance.AdvanceTime();
			factionPressureModifier.AdvanceTime();
			factionPressureThreshold.AdvanceTime();
		}

		public static FactionStats operator +(FactionStats a, FactionStats b)
		{
			FactionStats c = new FactionStats();

			c.actionsPerTurn = a.actionsPerTurn + b.actionsPerTurn;

			c.birthRate = a.birthRate + b.birthRate;
			c.deathRate = a.deathRate + b.deathRate;
			c.foodProductionPerWorker = a.foodProductionPerWorker + b.foodProductionPerWorker;
			c.spoilageRate = a.spoilageRate + b.spoilageRate;
			c.maxFoodByLand = a.maxFoodByLand + b.maxFoodByLand;
			c.maxBurgeoningTension = a.maxBurgeoningTension + b.maxBurgeoningTension;
			c.rebellionChance = a.rebellionChance + b.rebellionChance;
			c.factionPressureModifier = a.factionPressureModifier + b.factionPressureModifier;
			c.factionPressureThreshold = a.factionPressureThreshold + b.factionPressureThreshold;

			return c;
		}
	}
}