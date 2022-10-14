using System.Collections.Generic;
using UnityEngine;

namespace Game.Terrain
{
	//Taiga, Tundra, Desert, Tropical Rainforest, Temperate Forest, Polar
	public enum BiomeTerrainType { Swamp, Grassland, Forest, Rainforest, Desert, Taiga, Tundra, Shrubland, Badlands, Polar, Reef, Ocean, Deep_Ocean };
	public class Biome
	{
		static float[] temperatureBands = { 0.1f, 0.3f, 0.6f };

		static float[] moistureBands = { 0.12f, 0.28f, 0.85f };

		static BiomeTerrainType[] terrainTypes =
		{
			BiomeTerrainType.Polar, BiomeTerrainType.Tundra, BiomeTerrainType.Tundra, BiomeTerrainType.Taiga,
			BiomeTerrainType.Shrubland, BiomeTerrainType.Shrubland, BiomeTerrainType.Grassland, BiomeTerrainType.Forest,
			BiomeTerrainType.Badlands, BiomeTerrainType.Grassland, BiomeTerrainType.Forest, BiomeTerrainType.Rainforest,
			BiomeTerrainType.Desert, BiomeTerrainType.Grassland, BiomeTerrainType.Rainforest, BiomeTerrainType.Swamp
		};

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

				//calculate special cases for terrain type changes based on elevation and prox to rivers
				//then calculate for ocean biomes

				int rockDesertElevation =
				elevationMaximum - (elevationMaximum - waterLevel) / 2;

				var terrainType = terrainTypes[t * 4 + m];

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

				var plantMod = 0;

				if (BiomeInfo[terrainType].y < 3 && cell.HasRiver)
				{
					plantMod = 1;
				}

				return terrainType;
			}
			else
			{
				var terrainType = BiomeTerrainType.Ocean;

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

				return terrainType;
			}
		}
	}
}