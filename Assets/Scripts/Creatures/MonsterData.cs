using Game.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Creatures
{
	[CreateAssetMenu(fileName = nameof(MonsterData), menuName = "ScriptableObjects/Creatures/" + nameof(MonsterData), order = 1)]
	public class MonsterData : ScriptableObject
	{
		public string name;
		public string Name => name;
		//legendary
		public bool legendary;
		//land dwelling
		public bool landDwelling;
		//size
		public CreatureSize size;
		//type
		public CreatureType type;
		//alignment
		public CreatureAlignment alignment;
		//cr
		public float challengeRating;
		//xp
		public float experienceYield;
		//ac
		public string armorValue;
		//hp
		public string health;
		//speed
		public string speed;
		//stats
		public SerializableStatBlock stats;
		//saving throws
		public SerializableStatBlock savingThrows;
		//damage resistances
		public List<DamageType> damageResistances;
		//damage immunities
		public List<DamageType> damageImmunities;
		//condition immunities
		public List<ConditionType> conditionImmunities;
		//skills
		public List<string> skills;
		//senses
		public string senses;
		public string groupingName;
		public List<string> sounds;
		//languages
		public List<string> languages;
		//abilities
		public List<string> abilities;
		//actions
		public List<string> actions;
		//actions
		public List<string> legendaryActions;
	}
}