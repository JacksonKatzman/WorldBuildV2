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

		private Dictionary<TroopType, RandomWeightedPair> recruitmentWeights;
		private int recruitmentWeight;
		private int disbandWeight;


		public int Count => GetTotalTroops();

		public Military() : this(0,0,0)
		{

		}

		public Military(int weapons, int armor, int mounts, Dictionary<TroopType, RandomWeightedPair> recruitmentWeights = null)
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
				var randomRatio = SimRandom.RandomRange(0, recruitmentWeight);
				foreach(var pair in recruitmentWeights)
				{
					if(randomRatio - pair.Value.weight <= 0)
					{
						troops[pair.Key]++;
						break;
					}

					randomRatio -= pair.Value.weight;
				}
			}
		}

		private void DecreaseTroopCount(int amount)
		{
			for (int index = 0; index < amount; index++)
			{
				var randomRatio = SimRandom.RandomRange(0, disbandWeight);
				foreach (var pair in recruitmentWeights)
				{
					if (randomRatio - pair.Value.inverse <= 0 && troops[pair.Key] > 0)
					{
						troops[pair.Key]--;
						break;
					}

					randomRatio -= pair.Value.weight;
				}
			}
		}

		private void GenerateMilitaryStrategy()
		{
			recruitmentWeights = new Dictionary<TroopType, RandomWeightedPair>();
			foreach (TroopType troopType in (TroopType[])Enum.GetValues(typeof(TroopType)))
			{
				recruitmentWeights.Add(troopType, new RandomWeightedPair(1, 10));
			}

			foreach(RandomWeightedPair pair in recruitmentWeights.Values)
			{
				recruitmentWeight += pair.weight;
				disbandWeight += pair.inverse;
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