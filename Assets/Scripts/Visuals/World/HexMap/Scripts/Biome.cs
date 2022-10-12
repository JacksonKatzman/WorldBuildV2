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

		static Dictionary<BiomeTerrainType, Vector2Int> values = new Dictionary<BiomeTerrainType, Vector2Int>
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

		public BiomeTerrainType TerrainType { get; private set; }
		public int TerrainTypeTextureValue => values[TerrainType].x;
		public int VegetationLevel => values[TerrainType].y;

		public float Temperature { get; private set; }
		public float MoistureLevel { get; private set; }

		public Biome(HexCell cell, float temperature, float moistureLevel, int elevationMaximum)
		{
			Temperature = temperature;
			MoistureLevel = moistureLevel;

			CalculateTerrainType(cell, elevationMaximum);
		}

		private BiomeTerrainType CalculateTerrainType(HexCell cell, int elevationMaximum)
		{
			if (!cell.IsUnderwater)
			{
				int t = 0;
				for (; t < temperatureBands.Length; t++)
				{
					if (Temperature < temperatureBands[t])
					{
						break;
					}
				}
				int m = 0;
				for (; m < moistureBands.Length; m++)
				{
					if (MoistureLevel < moistureBands[m])
					{
						break;
					}
				}

				//calculate special cases for terrain type changes based on elevation and prox to rivers
				//then calculate for ocean biomes


				//HexBiome cellBiome = biomes[t * 4 + m];
				/*
				if (cellBiome.terrain == 0)
				{
					if (cell.Elevation >= rockDesertElevation)
					{
						cellBiome.terrain = 3;
					}
				}
				else if (cell.Elevation == elevationMaximum)
				{
					cellBiome.terrain = 4;
				}

				if (cellBiome.terrain == 4)
				{
					cellBiome.plant = 0;
				}
				else if (cellBiome.plant < 3 && cell.HasRiver)
				{
					cellBiome.plant += 1;
				}

				cell.TerrainTypeIndex = cellBiome.terrain;
				cell.PlantLevel = cellBiome.plant;
				*/
			}

			return BiomeTerrainType.Swamp;
		}
	}
}