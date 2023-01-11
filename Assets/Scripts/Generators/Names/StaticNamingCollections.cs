using Game.Terrain;
using System.Collections.Generic;

namespace Game.Generators.Names
{
	public static class StaticNamingCollections
	{
		public static List<WeightedString> VOWELS = new List<WeightedString>()
		{
			new WeightedString("aa", 1, true, true), new WeightedString("ui", 1, true, true),
			new WeightedString("ai", 1, true, true), new WeightedString("uo", 1, true, true),
			new WeightedString("ao", 1, true, true), new WeightedString("uu", 1, false, true),
			new WeightedString("au", 1, true, true), new WeightedString("uy", 1, true, true),
			new WeightedString("ei", 1, true, true), new WeightedString("y", 2, false, true),
			new WeightedString("eo", 1, true, true), new WeightedString("ay", 2, true, true),
			new WeightedString("eu", 1, true, true), new WeightedString("ea", 2, true, true),
			new WeightedString("ie", 1, false, true), new WeightedString("ey", 2, true, true),
			new WeightedString("ii", 1, false, true), new WeightedString("ia", 2, false, true),
			new WeightedString("io", 1, true, true), new WeightedString("ou", 2, true, true),
			new WeightedString("iu", 1, true, true), new WeightedString("ae", 3, true, true),
			new WeightedString("iy", 1, true, true), new WeightedString("ee", 3, false, true),
			new WeightedString("oa", 1, true, true), new WeightedString("oo", 3, false, true),
			new WeightedString("oe", 1, true, true), new WeightedString("o", 9, true, true),
			new WeightedString("oi", 1, true, true), new WeightedString("u", 9, true, true),
			new WeightedString("oy", 1, true, true), new WeightedString("i", 10, true, true),
			new WeightedString("ua", 1, false, true), new WeightedString("a", 12, true, true),
			new WeightedString("ue", 1, false, true), new WeightedString("e", 12, true, true),
		};

		public static List<WeightedString> CONSONANTS = new List<WeightedString>()
		{
			new WeightedString("x", 1, true, true), new WeightedString("gl", 2, true, false),
			new WeightedString("g", 2, true, true), new WeightedString("gr", 2, true, false),
			new WeightedString("h", 2, true, true), new WeightedString("ng", 3, false, true),
			new WeightedString("j", 2, true, true), new WeightedString("ph", 1, true, true),
			new WeightedString("q", 2, true, true), new WeightedString("pl", 2, true, false),
			new WeightedString("y", 2, true, true), new WeightedString("pr", 2, true, false),
			new WeightedString("b", 2, true, true), new WeightedString("qu", 1, true, false),
			new WeightedString("f", 3, true, true), new WeightedString("sc", 3, true, false),
			new WeightedString("p", 3, true, true), new WeightedString("sh", 3, true, true),
			new WeightedString("v", 3, true, true), new WeightedString("sk", 2, true, true),
			new WeightedString("w", 3, true, true), new WeightedString("sl", 2, true, false),
			new WeightedString("z", 3, true, true), new WeightedString("sm", 1, true, false),
			new WeightedString("d", 4, true, true), new WeightedString("sn", 2, true, false),
			new WeightedString("k", 4, true, true), new WeightedString("sp", 2, true, true),
			new WeightedString("c", 5, true, true), new WeightedString("st", 3, true, true),
			new WeightedString("l", 5, true, true), new WeightedString("sw", 2, true, false),
			new WeightedString("m", 5, true, true), new WeightedString("th", 4, true, true),
			new WeightedString("s", 6, true, true), new WeightedString("tr", 2, true, false),
			new WeightedString("r", 8, true, true), new WeightedString("tw", 1, true, false),
			new WeightedString("n", 9, true, true), new WeightedString("wh", 1, true, false),
			new WeightedString("t", 9, true, true), new WeightedString("wr", 2, true, false),
			new WeightedString("bl", 2, true, false), new WeightedString("nth", 1, false, true),
			new WeightedString("br", 3, true, false), new WeightedString("sch", 1, true, true),
			new WeightedString("ch", 3, true, true), new WeightedString("scr", 1, true, false),
			new WeightedString("ck", 3, false, true), new WeightedString("shr", 1, true, false),
			new WeightedString("cl", 3, true, false), new WeightedString("spl", 1, true, false),
			new WeightedString("cr", 2, true, false), new WeightedString("spr", 1, true, false),
			new WeightedString("dr", 2, true, false), new WeightedString("squ", 1, true, false),
			new WeightedString("fl", 2, true, false), new WeightedString("str", 1, true, false),
			new WeightedString("fr", 2, true, false), new WeightedString("thr", 1, true, false),
			new WeightedString("gh", 1, true, true), new WeightedString("rf", 3, false, true)
		};

		public static Dictionary<BiomeTerrainType, List<WeightedString>> TERRAIN_TYPES = new Dictionary<BiomeTerrainType, List<WeightedString>>()
		{
			{ BiomeTerrainType.Badlands, BADLANDS_FORMATS },
			{ BiomeTerrainType.Desert, DESERT_FORMATS },
			{ BiomeTerrainType.Forest, FOREST_FORMATS },
			{ BiomeTerrainType.Grassland, GRASSLAND_FORMATS },
			{ BiomeTerrainType.Ocean, OCEAN_FORMATS },
			{ BiomeTerrainType.Deep_Ocean, OCEAN_FORMATS },
			{ BiomeTerrainType.Polar, POLAR_FORMATS },
			{ BiomeTerrainType.Rainforest, RAINFOREST_FORMATS },
			{ BiomeTerrainType.Reef, REEF_FORMATS },
			{ BiomeTerrainType.Shrubland, SHRUBLAND_FORMATS },
			{ BiomeTerrainType.Swamp, SWAMP_FORMATS },
			{ BiomeTerrainType.Taiga, TAIGA_FORMATS },
			{ BiomeTerrainType.Tundra, TUNDRA_FORMATS }
		};

		private static List<WeightedString> BADLANDS_FORMATS = new List<WeightedString>()
		{
			new WeightedString("The {A} Wastes", 1, true, true),
			new WeightedString("The {A}lands", 1, true, true),
			new WeightedString("{A} Canyons", 1, true, true),
			new WeightedString("{A} Mesas", 1, true, true)
		};

		private static List<WeightedString> DESERT_FORMATS = new List<WeightedString>()
		{
			new WeightedString("{A} Dunes", 1, true, true),
			new WeightedString("{A} Expanse", 1, true, true),
			new WeightedString("The {A} Desert", 1, true, true)
		};

		private static List<WeightedString> FOREST_FORMATS = new List<WeightedString>()
		{
			new WeightedString("The {A}wood", 1, true, true),
			new WeightedString("{A} Forest", 1, true, true)
		};

		private static List<WeightedString> GRASSLAND_FORMATS = new List<WeightedString>()
		{
			new WeightedString("The {A}ways", 1, true, true),
			new WeightedString("Plains of {S}", 1, true, true),
			new WeightedString("The {S} Grass", 1, true, true)
		};

		private static List<WeightedString> OCEAN_FORMATS = new List<WeightedString>()
		{
			new WeightedString("The {A} Sea", 1, true, true),
			new WeightedString("The {A} Ocean", 1, true, true)
		};

		private static List<WeightedString> POLAR_FORMATS = new List<WeightedString>()
		{
			new WeightedString("The {A}-Ice Sheets", 1, true, true)
		};

		private static List<WeightedString> RAINFOREST_FORMATS = new List<WeightedString>()
		{
			new WeightedString("{A} Jungle", 1, true, true)
		};

		private static List<WeightedString> REEF_FORMATS = new List<WeightedString>()
		{
			new WeightedString("{A} Reef", 1, true, true)
		};

		private static List<WeightedString> SHRUBLAND_FORMATS = new List<WeightedString>()
		{
			new WeightedString("{A} Brushlands", 1, true, true),
			new WeightedString("{A} Shrublands", 1, true, true)
		};

		private static List<WeightedString> SWAMP_FORMATS = new List<WeightedString>()
		{
			new WeightedString("{S}'s Mire", 1, true, true),
			new WeightedString("{A} Bog", 1, true, true),
			new WeightedString("{A} Swamp", 1, true, true)
		};

		private static List<WeightedString> TAIGA_FORMATS = new List<WeightedString>()
		{
			new WeightedString("{A}peaks", 1, true, true)
		};

		private static List<WeightedString> TUNDRA_FORMATS = new List<WeightedString>()
		{
			new WeightedString("{S} Frostlands", 1, true, true)
		};
	}
}
