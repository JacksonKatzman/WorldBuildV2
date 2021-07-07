using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Generators.Noise;
using Game.Enums;
using Game.Factions;

namespace Game.WorldGeneration
{
    public class World : ITimeSensitive
    {
		public Dictionary<MapCategory, float[,]> noiseMaps;
		public Color[,] biomeMap;

		public Texture2D heightMapTexture;
		public Texture2D colorMapTexture;

		private List<Biome> biomes;
		public List<Faction> factions;

		NoiseSettings noiseSettings;
        private Chunk[,] worldChunks;
		public int chunkSize;
		public int yearsPassed;

		private List<Faction> temporaryFactionContainer;

		public World(float[,] noiseMap, Texture2D texture, NoiseSettings settings, int chunkSize, List<Biome> biomes)
		{
			noiseMaps = new Dictionary<MapCategory, float[,]>();
			factions = new List<Faction>();
			temporaryFactionContainer = new List<Faction>();

			AlterMap(MapCategory.TERRAIN, noiseMap);
			heightMapTexture = texture;
			noiseSettings = settings;
			this.chunkSize = chunkSize;
			this.biomes = biomes;

			Setup();
			BuildChunks();
			CreateBiomeMap();
		}

		public float[,] SampleNoiseMap(MapCategory mapCategory, Vector2Int chunkLocation)
		{
			var storedMap = noiseMaps[mapCategory];
			float[,] sampledMap = new float[chunkSize, chunkSize];
			for(int y = 0; y < chunkSize; y++)
			{
				for(int x = 0; x < chunkSize; x++)
				{
					var modifiedX = (chunkSize * chunkLocation.x) + x;
					var modifiedY = (chunkSize * chunkLocation.y) + y;
					sampledMap[x, y] = storedMap[modifiedX, modifiedY];
				}
			}
			return sampledMap;
		}

		public void AdvanceTime()
		{
			GenerateNewNoiseMap(MapCategory.RAINFALL);

			foreach(Chunk chunk in worldChunks)
			{
				chunk.AdvanceTime();
			}

			foreach(Faction faction in factions)
			{
				faction.AdvanceTime();
			}

			HandleDeferredItems();

			yearsPassed++;
		}

		public Biome CalculateTileBiome(LandType landType, float rainfall, float fertility)
		{
			return Biome.CalculateBiomeType(biomes, landType, rainfall, fertility);
		}

		public void SpawnRandomCity()
		{
			bool spawned = false;
			while (!spawned)
			{
				var randomXIndex = WorldHandler.Instance.RandomRange(0, worldChunks.GetLength(0));
				var randomYIndex = WorldHandler.Instance.RandomRange(0, worldChunks.GetLength(1));
				spawned = worldChunks[randomXIndex, randomYIndex].SpawnCity(100, 100);
			}
			HandleDeferredItems();
		}

		public void CreateNewFaction(Tile tile, float food, int population)
		{
			var faction = new Faction(tile, food, population);
			temporaryFactionContainer.Add(faction);
		}

		public Tile GetTileAtWorldPosition(Vector2Int worldPosition)
		{
			Vector2Int chunkCoords = new Vector2Int(worldPosition.x / chunkSize, worldPosition.y / chunkSize);
			return worldChunks[chunkCoords.x, chunkCoords.y].chunkTiles[worldPosition.x % chunkSize, worldPosition.y % chunkSize];
		}

		public Faction GetFactionThatControlsTile(Tile tile)
		{
			Faction controllingFaction = null;
			foreach(Faction faction in factions)
			{
				if(faction.territory.Contains(tile))
				{
					controllingFaction = faction;
					break;
				}
			}

			return controllingFaction;
		}

		private void BuildChunks()
		{
			worldChunks = new Chunk[noiseSettings.worldSize.x, noiseSettings.worldSize.y];
			for (int y = 0; y < noiseSettings.worldSize.y; y++)
			{
				for (int x = 0; x < noiseSettings.worldSize.x; x++)
				{
					worldChunks[x, y] = new Chunk(this, new Vector2Int(x, y));
				}
			}
		}

		private void AlterMap(MapCategory mapCategory, float[,] noiseMap)
		{
			if(!noiseMaps.ContainsKey(mapCategory))
			{
				noiseMaps.Add(mapCategory, null);
			}
			noiseMaps[mapCategory] = noiseMap;
		}

		private void Setup()
		{
			GenerateNewNoiseMap(MapCategory.RAINFALL);
			GenerateNewNoiseMap(MapCategory.FERTILITY);

			yearsPassed = 0;
		}

		private void GenerateNewNoiseMap(MapCategory category)
		{
			Vector2Int noiseSize = new Vector2Int(noiseSettings.worldSize.x * chunkSize, noiseSettings.worldSize.y * chunkSize);
			var map = NoiseGenerator.GeneratePerlinNoise(noiseSize, noiseSettings.scale,
														  noiseSettings.octaves, noiseSettings.persistance,
														  noiseSettings.lacunarity, noiseSettings.offset);
			AlterMap(category, map);
		}

		private void CreateBiomeMap()
		{
			var width = worldChunks.GetLength(0) * chunkSize;
			var height = worldChunks.GetLength(1) * chunkSize;
			biomeMap = new Color[width, height];

			foreach(Chunk chunk in worldChunks)
			{
				var chunkColorMap = chunk.GetBiomeColorMap();
				var chunkColorMapWidth = chunkColorMap.GetLength(0);
				var chunkColorMapHeight = chunkColorMap.GetLength(1);
				for(int y = 0; y < chunkColorMapHeight; y++)
				{
					for(int x = 0; x < chunkColorMapWidth; x++)
					{
						var modPosX = x + (chunkSize * chunk.coords.x);
						var modPosY = y + (chunkSize * chunk.coords.y);

						biomeMap[modPosX, modPosY] = chunkColorMap[x, y];
					}
				}
			}
			Color[] colorMap = new Color[width * height];
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					colorMap[y * width + x] = biomeMap[x,y];
				}
			}
			colorMapTexture = new Texture2D(width, height);
			colorMapTexture.SetPixels(colorMap);
			colorMapTexture.Apply();
		}

		private void HandleDeferredItems()
		{
			foreach(Faction faction in temporaryFactionContainer)
			{
				factions.Add(faction);
			}
			temporaryFactionContainer.Clear();
		}
	}
}
