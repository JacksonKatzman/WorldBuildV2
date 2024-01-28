using System.Collections.Generic;
using UnityEngine;

namespace Game.Terrain
{
    //Taiga, Tundra, Desert, Tropical Rainforest, Temperate Forest, Polar
    public enum BiomeTerrainType { Swamp, Grassland, Forest, Rainforest, Desert, Taiga, Tundra, Shrubland, Badlands, Polar, Reef, Ocean, Deep_Ocean };
	public class Biome
	{
		static float[] temperatureBands = { 0.1f, 0.3f, 0.6f, 10.0f };

		static float[] moistureBands = { 0.12f, 0.28f, 0.85f, 10.0f };

		static float[] elevationBands = { 0.1f, 0.25f, 0.55f, 10.0f };

		public static Dictionary<BiomeTerrainType, Vector2Int> BiomeInfo = new Dictionary<BiomeTerrainType, Vector2Int>
		{
			{BiomeTerrainType.Swamp, new Vector2Int(0,2)},
			{BiomeTerrainType.Grassland, new Vector2Int(1,1)},
			{BiomeTerrainType.Forest, new Vector2Int(2,3)},
			{BiomeTerrainType.Rainforest, new Vector2Int(3,3)},
			{BiomeTerrainType.Desert, new Vector2Int(4,0)},
			{BiomeTerrainType.Taiga, new Vector2Int(5,2)},
			{BiomeTerrainType.Tundra, new Vector2Int(6,1)},
			{BiomeTerrainType.Shrubland, new Vector2Int(7,1)},
			{BiomeTerrainType.Badlands, new Vector2Int(8,0)},
			{BiomeTerrainType.Polar, new Vector2Int(9,0)},
			{BiomeTerrainType.Reef, new Vector2Int(2,2)},
			{BiomeTerrainType.Ocean, new Vector2Int(4,0)},
			{BiomeTerrainType.Deep_Ocean, new Vector2Int(1,0)}
		};

		public static Dictionary<BiomeTerrainType, List<BiomeTerrainType>> BiomeMatches = new Dictionary<BiomeTerrainType, List<BiomeTerrainType>>
		{
			{BiomeTerrainType.Swamp, new List<BiomeTerrainType>() { BiomeTerrainType.Swamp } },
			{BiomeTerrainType.Grassland, new List<BiomeTerrainType>() { BiomeTerrainType.Grassland, BiomeTerrainType.Shrubland, BiomeTerrainType.Tundra } },
			{BiomeTerrainType.Forest, new List<BiomeTerrainType>() { BiomeTerrainType.Forest, BiomeTerrainType.Rainforest, BiomeTerrainType.Taiga } },
			{BiomeTerrainType.Rainforest, new List<BiomeTerrainType>() { BiomeTerrainType.Rainforest, BiomeTerrainType.Forest } },
			{BiomeTerrainType.Desert, new List<BiomeTerrainType>() { BiomeTerrainType.Desert, BiomeTerrainType.Shrubland, BiomeTerrainType.Badlands } },
			{BiomeTerrainType.Taiga, new List<BiomeTerrainType>() { BiomeTerrainType.Taiga, BiomeTerrainType.Polar } },
			{BiomeTerrainType.Tundra, new List<BiomeTerrainType>() { BiomeTerrainType.Tundra, BiomeTerrainType.Shrubland, BiomeTerrainType.Polar } },
			{BiomeTerrainType.Shrubland, new List<BiomeTerrainType>() { BiomeTerrainType.Shrubland, BiomeTerrainType.Grassland } },
			{BiomeTerrainType.Badlands, new List<BiomeTerrainType>() { BiomeTerrainType.Badlands, BiomeTerrainType.Desert } },
			{BiomeTerrainType.Polar, new List<BiomeTerrainType>() { BiomeTerrainType.Polar, BiomeTerrainType.Tundra, BiomeTerrainType.Taiga } },
			{BiomeTerrainType.Reef, new List<BiomeTerrainType>() { BiomeTerrainType.Reef, BiomeTerrainType.Ocean, BiomeTerrainType.Deep_Ocean } },
			{BiomeTerrainType.Ocean, new List<BiomeTerrainType>() { BiomeTerrainType.Ocean, BiomeTerrainType.Reef, BiomeTerrainType.Deep_Ocean } },
			{BiomeTerrainType.Deep_Ocean, new List<BiomeTerrainType>() { BiomeTerrainType.Deep_Ocean, BiomeTerrainType.Reef, BiomeTerrainType.Ocean } }
		};

		public static Dictionary<int, List<BiomeTerrainType>> BiomesByElevation = new Dictionary<int, List<BiomeTerrainType>>
		{
			{ 0, new List<BiomeTerrainType> {
			BiomeTerrainType.Tundra, BiomeTerrainType.Tundra, BiomeTerrainType.Polar, BiomeTerrainType.Polar,
			BiomeTerrainType.Polar, BiomeTerrainType.Tundra, BiomeTerrainType.Tundra, BiomeTerrainType.Polar,
			BiomeTerrainType.Desert, BiomeTerrainType.Shrubland, BiomeTerrainType.Grassland, BiomeTerrainType.Swamp,
			BiomeTerrainType.Desert, BiomeTerrainType.Desert, BiomeTerrainType.Grassland, BiomeTerrainType.Swamp } },
			{ 1, new List<BiomeTerrainType> {
			BiomeTerrainType.Tundra, BiomeTerrainType.Tundra, BiomeTerrainType.Polar, BiomeTerrainType.Polar,
			BiomeTerrainType.Polar, BiomeTerrainType.Tundra, BiomeTerrainType.Tundra, BiomeTerrainType.Polar,
			BiomeTerrainType.Shrubland, BiomeTerrainType.Grassland, BiomeTerrainType.Grassland, BiomeTerrainType.Forest,
			BiomeTerrainType.Desert, BiomeTerrainType.Shrubland, BiomeTerrainType.Grassland, BiomeTerrainType.Rainforest} },
			{ 2, new List<BiomeTerrainType> {
			BiomeTerrainType.Polar, BiomeTerrainType.Polar, BiomeTerrainType.Taiga, BiomeTerrainType.Taiga,
			BiomeTerrainType.Polar, BiomeTerrainType.Badlands, BiomeTerrainType.Taiga, BiomeTerrainType.Taiga,
			BiomeTerrainType.Badlands, BiomeTerrainType.Badlands, BiomeTerrainType.Forest, BiomeTerrainType.Forest,
			BiomeTerrainType.Badlands, BiomeTerrainType.Badlands, BiomeTerrainType.Rainforest, BiomeTerrainType.Rainforest} },
			{ 3, new List<BiomeTerrainType> { 
			BiomeTerrainType.Badlands, BiomeTerrainType.Badlands, BiomeTerrainType.Taiga, BiomeTerrainType.Taiga,
			BiomeTerrainType.Badlands, BiomeTerrainType.Badlands, BiomeTerrainType.Taiga, BiomeTerrainType.Taiga,
			BiomeTerrainType.Badlands, BiomeTerrainType.Badlands, BiomeTerrainType.Forest, BiomeTerrainType.Forest,
			BiomeTerrainType.Badlands, BiomeTerrainType.Badlands, BiomeTerrainType.Badlands, BiomeTerrainType.Badlands} }
		};

		public static BiomeTerrainType CalculateTerrainType(HexCell cell, float temperature, float moistureLevel, int elevationMaximum, int waterLevel)
		{
			if (!cell.IsUnderwater)
			{
				int t = 0;
				for (; t < temperatureBands.Length; t++)
				{
					if (temperature < temperatureBands[t])
					{
						break;
					}
				}

				int m = 0;
				for (; m < moistureBands.Length; m++)
				{
					if (moistureLevel < moistureBands[m])
					{
						break;
					}
				}

				int e = 0;
				for (; e < elevationBands.Length; e++)
				{
					if (cell.Elevation/(elevationMaximum - waterLevel) < elevationBands[e])
					{
						break;
					}
				}
				//calculate special cases for terrain type changes based on elevation and prox to rivers
				//then calculate for ocean biomes

				int rockDesertElevation =
				elevationMaximum - (elevationMaximum - waterLevel) / 2;

				var ele = BiomesByElevation[e];
				var terrainType = ele[t * 4 + m];

				/*
				if (terrainType == BiomeTerrainType.Desert || terrainType == BiomeTerrainType.Shrubland)
				{
					if (cell.Elevation >= rockDesertElevation)
					{
						terrainType = BiomeTerrainType.Badlands;
					}
				}
				else if (cell.Elevation == elevationMaximum)
				{
					terrainType = BiomeTerrainType.Polar;
				}
				*/
				var plantMod = 0;

				if (BiomeInfo[terrainType].y < 3 && cell.HasRiver)
				{
					plantMod = 1;
				}

				cell.PlantLevel = plantMod;
				return terrainType;
			}
			else
			{
				var terrainType = BiomeTerrainType.Ocean;
				/*
				if (cell.Elevation == waterLevel - 1)
				{
					int cliffs = 0, slopes = 0;
					for (
						HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++
					)
					{
						HexCell neighbor = cell.GetNeighbor(d);
						if (!neighbor)
						{
							continue;
						}
						int delta = neighbor.Elevation - cell.WaterLevel;
						if (delta == 0)
						{
							slopes += 1;
						}
						else if (delta > 0)
						{
							cliffs += 1;
						}
					}

					if (cliffs + slopes > 3)
					{
						terrainType = BiomeTerrainType.Reef;
					}
					else if (cliffs > 0)
					{
						terrainType = BiomeTerrainType.Deep_Ocean;
					}
					else if (slopes > 0)
					{
						terrainType = BiomeTerrainType.Ocean;
					}
					else
					{
						terrainType = BiomeTerrainType.Ocean;
					}
				}
				else if (cell.Elevation >= waterLevel)
				{
					terrainType = BiomeTerrainType.Reef;
				}
				else if (cell.Elevation < 0)
				{
					terrainType = BiomeTerrainType.Deep_Ocean;
				}
				else
				{
					terrainType = BiomeTerrainType.Ocean;
				}

				if (terrainType == BiomeTerrainType.Ocean && temperature < temperatureBands[0])
				{
					terrainType = BiomeTerrainType.Deep_Ocean;
				}
				*/

				return terrainType;
			}
		}
	}
}