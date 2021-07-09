using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;
using System;

namespace Game.Factions
{
	public class Military
	{
		public Dictionary<TroopType, int> troops;
		public int weapons;
		public int armor;
		public int mounts;

		private Dictionary<TroopType, WeightRatioPair> recruitmentWeights;

		public int Count => GetTotalTroops();

		public Military() : this(0,0,0)
		{

		}

		public Military(int weapons, int armor, int mounts, Dictionary<TroopType, WeightRatioPair> recruitmentWeights = null)
		{
			this.weapons = weapons;
			this.armor = armor;
			this.mounts = mounts;
			this.recruitmentWeights = recruitmentWeights;

			troops = new Dictionary<TroopType, int>();
			foreach (TroopType troopType in (TroopType[])Enum.GetValues(typeof(TroopType)))
			{
				troops.Add(troopType, 0);
			}

			if(recruitmentWeights == null)
			{
				GenerateMilitaryStrategy();
			}
		}

		public void ModifyTroopCount(int amount)
		{
			if(amount > 0)
			{
				IncreaseTroopCount(amount);
			}
			else
			{
				amount = Mathf.Clamp(Mathf.Abs(amount), 0, Count);
				DecreaseTroopCount(amount);
			}
		}

		public void ModifySupplies(int weapons, int armor, int mounts)
		{
			this.weapons = Mathf.Clamp(this.weapons + weapons, 0, int.MaxValue);
			this.armor = Mathf.Clamp(this.armor + armor, 0, int.MaxValue);
			this.mounts = Mathf.Clamp(this.mounts + mounts, 0, int.MaxValue);
		}
		
		private void IncreaseTroopCount(int amount)
		{
			for (int index = 0; index < amount; index++)
			{
				var currentBestFit = TroopType.LIGHT_INFANTRY;
				var currentLowestScore = float.MaxValue;
				var troopCount = Count;

				foreach (var pair in troops)
				{
					var score = (pair.Value / troopCount) - recruitmentWeights[pair.Key].ratio;
					if (score < currentLowestScore)
					{
						currentBestFit = pair.Key;
						currentLowestScore = score;
					}
				}

				troops[currentBestFit]++;
			}
		}

		private void DecreaseTroopCount(int amount)
		{
			for (int index = 0; index < amount; index++)
			{
				var currentBestFit = TroopType.LIGHT_INFANTRY;
				var currentHighestScore = float.MinValue;
				var troopCount = Count;

				foreach (var pair in troops)
				{
					var score = (pair.Value / troopCount) - recruitmentWeights[pair.Key].ratio;
					if (score > currentHighestScore)
					{
						currentBestFit = pair.Key;
						currentHighestScore = score;
					}
				}

				troops[currentBestFit]--;
			}
		}

		private void GenerateMilitaryStrategy()
		{
			recruitmentWeights = new Dictionary<TroopType, WeightRatioPair>();
			foreach (TroopType troopType in (TroopType[])Enum.GetValues(typeof(TroopType)))
			{
				recruitmentWeights.Add(troopType, new WeightRatioPair(SimRandom.RandomRange(1,10), 0.0f));
			}

			var totalWeight = 0;
			foreach(WeightRatioPair pair in recruitmentWeights.Values)
			{
				totalWeight += pair.weight;
			}
			foreach (WeightRatioPair pair in recruitmentWeights.Values)
			{
				pair.ratio = pair.weight / totalWeight;
			}
		}

		private int GetTotalTroops()
		{
			int total = 0;
			foreach(int count in troops.Values)
			{
				total += count;
			}
			return total;
		}
	}
}