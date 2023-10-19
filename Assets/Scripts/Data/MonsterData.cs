using Game.Data;
using Game.Enums;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
	[CreateAssetMenu(fileName = nameof(MonsterData), menuName = "ScriptableObjects/Creatures/" + nameof(MonsterData), order = 1)]
	public class MonsterData : SerializedScriptableObject
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
		//prof
		public int proficiencyBonus;
		//ac
		public int armorValue;
		//ac type
		public string armorType;
		//hp
		public int health;
		//speed
		public int speed;
		public int climbSpeed;
		public int swimSpeed;
		public int flySpeed;
		//passive perception
		public int passivePerception;
		//stats
		public SerializableStatBlock stats;
		//saving throws
		public SerializableStatBlock savingThrows;
		//damage vulnerabilities
		public List<DamageType> damageVulnerabilities;
		//damage resistances
		public List<DamageType> damageResistances;
		//damage immunities
		public List<DamageType> damageImmunities;
		//condition immunities
		public List<ConditionType> conditionImmunities;
		//skills
		public List<string> skills;
		//senses
		public List<SenseRange> senses;
		public string groupingName;
		public List<string> sounds;
		//languages
		public List<string> languages;
		//abilities
		public List<CreatureAbility> abilities;
		//actions
		public List<CreatureAction> actions;
		//actions
		public List<CreatureAction> legendaryActions;
		public Gender requiredGender = Gender.ANY;
	}

	[Serializable]
	public struct SenseRange
	{
		public SensesType senseType;
		public int distance;
	}

	[Serializable]
	public struct CreatureAbility
	{
		public string abilityName;
		[TextArea(2, 8)]
		public string abilityDescription;
	}

	[Serializable]
	public struct CreatureAction
	{
		public string actionName;
		[TextArea(2, 8)]
		public string actionDescription;
	}
}