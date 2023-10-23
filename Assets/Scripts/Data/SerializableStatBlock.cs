using Game.Enums;
using System.Collections.Generic;

namespace Game.Data
{
	[System.Serializable]
	public class SerializableStatBlock
	{
		public int strength;
		public int dexterity;
		public int constitution;
		public int intelligence;
		public int wisdom;
		public int charisma;
		public int luck;

		public SerializableStatBlock() { }

		public SerializableStatBlock(int strength, int dexterity, int constitution, int intelligence, int wisdom, int charisma, int luck)
		{
			this.strength = strength;
			this.dexterity = dexterity;
			this.constitution = constitution;
			this.intelligence = intelligence;
			this.wisdom = wisdom;
			this.charisma = charisma;
			this.luck = luck;
		}

		public Dictionary<StatType, int> BuildDictionary()
		{
			var dictionary = new Dictionary<StatType, int>();

			dictionary.Add(StatType.STRENGTH, strength);
			dictionary.Add(StatType.DEXTERITY, dexterity);
			dictionary.Add(StatType.CONSTITUTION, constitution);
			dictionary.Add(StatType.INTELLIGENCE, intelligence);
			dictionary.Add(StatType.WISDOM, wisdom);
			dictionary.Add(StatType.CHARISMA, charisma);
			dictionary.Add(StatType.LUCK, luck);

			return dictionary;
		}
	}
}