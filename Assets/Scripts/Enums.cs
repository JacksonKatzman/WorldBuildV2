using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Enums
{
	[System.Serializable]
	public enum Gender { MALE, FEMALE, ANY };
	public enum MapCategory { TERRAIN, RAINFALL, FERTILITY, BIOME };
	public enum LandType { OCEAN, FLAT, HILLS, MOUNTAINS };
	public enum PriorityType { MILITARY, INFRASTRUCTURE, MERCANTILE, POLITICAL, RELIGIOUS};
	public enum TroopType { LIGHT_INFANTRY, HEAVY_INFANTRY, LIGHT_CAVALRY, HEAVY_CAVALRY, ARCHER};
	public enum LogSource { CITY, NAMEGEN, WORLDGEN, MAIN, IMPORTANT, FACTION, FACTIONACTION, PEOPLE, PROFILE };
	public enum LogAllowance { ALL, SOME, NONE };
}