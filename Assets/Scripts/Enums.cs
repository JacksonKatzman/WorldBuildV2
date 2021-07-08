using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Enums
{
	[System.Serializable]
	public enum Gender { MALE, FEMALE, ANY };
	public enum MapCategory { TERRAIN, RAINFALL, FERTILITY, BIOME };
	public enum LandType { OCEAN, FLAT, HILLS, MOUNTAINS };
	public enum LogSource { CITY, NAMEGEN, WORLDGEN, MAIN, IMPORTANT, FACTION, FACTIONACTION, PEOPLE };
}