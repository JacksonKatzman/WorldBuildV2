using Game.Enums;
using System.Collections.Generic;

namespace Game.Data
{
	[System.Serializable]
	public class CreatureStats
	{
		public Dictionary<StatType, int> stats;

		public CreatureStats(int strength, int agility, int constitution, int intelligence, int wisdom, int charisma, int luck)
		{
			stats = new Dictionary<StatType, int>();

			stats.Add(StatType.STRENGTH, strength);
			stats.Add(StatType.DEXTERITY, agility);
			stats.Add(StatType.CONSTITUTION, constitution);
			stats.Add(StatType.INTELLIGENCE, intelligence);
			stats.Add(StatType.WISDOM, wisdom);
			stats.Add(StatType.CHARISMA, charisma);
			stats.Add(StatType.LUCK, luck);
		}

		public CreatureStats(Dictionary<StatType, int> stats)
		{
			this.stats = stats;
		}

		public CreatureStats(SerializableStatBlock block) : this(block.BuildDictionary())
		{
		}

		public static CreatureStats operator +(CreatureStats a, CreatureStats b)
		{
			var dictionary = new Dictionary<StatType, int>(a.stats);
			foreach (var pair in a.stats)
			{
				dictionary[pair.Key] += b.stats[pair.Key];
			}
			return new CreatureStats(dictionary);
		}

		public int GetFlatValue(StatType t)
		{
			return stats[t];
		}

		public int GetModValue(StatType t)
		{
			return (stats[t] - 10) / 2;
		}
	}
}