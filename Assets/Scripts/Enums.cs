using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Enums
{
	[System.Serializable]
	public enum Gender { MALE, FEMALE, ANY };
	public enum MapCategory { TERRAIN, RAINFALL, FERTILITY, BIOME };
	public enum LandType { OCEAN, FLAT, HILLS, MOUNTAINS, RIVER };
	public enum LandTemperature { HOT, TEMPERATE, COLD };
	public enum Direction { NORTH, SOUTH, EAST, WEST };
	public enum PriorityType { POLITICAL, ECONOMIC, RELIGIOUS, MILITARY, OTHER };
	public enum OrganizationType { POLITICAL, ECONOMIC, RELIGIOUS, MILITARY, OTHER };
	public enum TroopType { LIGHT_INFANTRY, HEAVY_INFANTRY, LIGHT_CAVALRY, HEAVY_CAVALRY, ARCHER};
	public enum StatType { STRENGTH, DEXTERITY, CONSTITUTION, INTELLIGENCE, WISDOM, CHARISMA, LUCK };
	public enum RoleType { GOVERNER, MAGIC_USER, ROGUE };
	public enum CreatureSize { TINY, SMALL, MEDIUM, LARGE, HUGE, GARGANTUAN};
	public enum CreatureType { ABERRATION, BEAST, CELESTIAL, CONSTRUCT, DRAGON, ELEMENTAL, FEY, FIEND, GIANT, HUMANOID, MONSTROSITY, OOZE, PLANT, UNDEAD };
	public enum DamageType { SLASHING, PIERCING, BLUDGEONING, POISON, ACID, FIRE, COLD, RADIANT, NECROTIC, LIGHTNING, THUNDER, FORCE, PSYCHIC, NONMAGICAL };
	public enum ConditionType { BLINDED, CHARMED, DEAFENED, FRIGHTENED, GRAPPLED, INCAPACITATED, INVISIBLE, PARALYZED, PETRIFIED, POISONED, PRONE, RESTRAINED, STUNNED, UNCONSCIOUS, EXHAUSTION };
	public enum SensesType {  BLINDSIGHT, DARKVISION, TREMORSENSE, TRUESIGHT }
	public enum CreatureAlignment { LAWFUL_GOOD, LAWFUL_NEUTRAL, LAWFUL_EVIL, NEUTRAL_GOOD, TRUE_NEUTRAL, NEUTRAL_EVIL, CHAOTIC_GOOD, CHAOTIC_NEUTRAL, CHAOTIC_EVIL, UNALIGNED, ANY_ALIGNMENT, GOOD, EVIL, LAWFUL, CHAOTIC };
	public enum CoinType { CP, SP, GP, PP };
	public enum WeaponCategory { SIMPLE_MELEE, SIMPLE_RANGED, MARTIAL_MELEE, MARTIAL_RANGED };
	public enum ArmorCategory { LIGHT, MEDIUM, HEAVY, SHIELD };
	public enum ItemGrade { AWFUL, POOR, NORMAL, GOOD, EXCELLENT, MASTERWORK, LEGENDARY };
	public enum LogSource { CITY, NAMEGEN, WORLDGEN, MAIN, IMPORTANT, FACTION, FACTIONACTION, PEOPLE, EVENT, PROFILE };
	public enum LogAllowance { ALL, SOME, NONE };
	public enum Disposition { PASSIVE, AGGRESSIVE };
	public enum EncounterLocationType { OVERWORLD, DUNGEON };
	public enum EncounterType { COMBAT, PUZZLE, ROLEPLAY, CURIOSITY, EXPLORATION, LONG };
	public enum HexEdgeType { Flat, Slope, Cliff };
	public enum LandmarkType { NONE, TOWER, STATUE };
	public enum FlavorType { SYNONYM, REASON };
	public enum ContextFamiliarity { UNKNOWN, AWARE, ACQUAINTED, KNOWLEDGEABLE, INTIMATE, TOTAL };

	public static class EnumExtensions
	{
		internal static IEnumerable<T> ToEnumerableOf<T>(this Enum theEnum)
		{
			return Enum.GetValues(theEnum.GetType()).Cast<T>();
		}

		public static string DependantPossessive(this Gender gender)
        {
			switch(gender)
            {
				case Gender.MALE:
					return "his";
				case Gender.FEMALE:
					return "her";
				default:
					return "their";
            }
        }

		public static string IndependantPossessive(this Gender gender)
		{
			switch (gender)
			{
				case Gender.MALE:
					return "his";
				case Gender.FEMALE:
					return "hers";
				default:
					return "theirs";
			}
		}

		public static string Object(this Gender gender)
		{
			switch (gender)
			{
				case Gender.MALE:
					return "him";
				case Gender.FEMALE:
					return "her";
				default:
					return "them";
			}
		}

		public static string Subject(this Gender gender)
		{
			switch (gender)
			{
				case Gender.MALE:
					return "he";
				case Gender.FEMALE:
					return "she";
				default:
					return "they";
			}
		}

		public static string SubjectContraction(this Gender gender)
		{
			switch (gender)
			{
				case Gender.MALE:
					return "he's";
				case Gender.FEMALE:
					return "she's";
				default:
					return "they're";
			}
		}
	}

	public enum HexDirection { NE, E, SE, SW, W, NW };
	public static class HexDirectionExtensions
	{
		public static HexDirection Opposite(this HexDirection direction)
		{
			return (int)direction < 3 ? (direction + 3) : (direction - 3);
		}
		public static HexDirection Previous(this HexDirection direction)
		{
			return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
		}

		public static HexDirection Next(this HexDirection direction)
		{
			return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
		}
		public static HexDirection Previous2(this HexDirection direction)
		{
			direction -= 2;
			return direction >= HexDirection.NE ? direction : (direction + 6);
		}

		public static HexDirection Next2(this HexDirection direction)
		{
			direction += 2;
			return direction <= HexDirection.NW ? direction : (direction - 6);
		}
	}
}