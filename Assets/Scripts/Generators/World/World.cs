using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Generators.Noise;
using Game.Enums;
using Game.Factions;
using System;
using Game.Data.EventHandling;

namespace Game.WorldGeneration
{
    public class World : ITimeSensitive
    {
		public Dictionary<MapCategory, float[,]> noiseMaps;
		public Color[,] biomeMap;
		public int Size => biomeMap.GetLength(0) * biomeMap.GetLength(1);

		public Texture2D heightMapTexture;
		public Texture2D colorMapTexture;
		public Texture2D voxelColorMapTexture;

		private List<Biome> biomes;
		public List<Faction> factions;

		NoiseSettings noiseSettings;
        private Chunk[,] worldChunks;
		public int chunkSize;
		public int yearsPassed;

		private List<Person> people;

		private List<Action> deferredActions;

		public World(float[,] noiseMap, Texture2D texture, NoiseSettings settings, int chunkSize, List<Biome> biomes)
		{
			noiseMaps = new Dictionary<MapCategory, float[,]>();
			factions = new List<Faction>();
			people = new List<Person>();
			deferredActions = new List<Action>();

			AlterMap(MapCategory.TERRAIN, noiseMap);
			heightMapTexture = texture;
			noiseSettings = settings;
			this.chunkSize = chunkSize;
			this.biomes = biomes;

			Setup();
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
			HandleDeferredActions();

			GenerateNewNoiseMap(MapCategory.RAINFALL);

			foreach(Chunk chunk in worldChunks)
			{
				chunk.AdvanceTime();
			}

			foreach (Faction faction in factions)
			{
				faction.currentPriorities = faction.GeneratePriorities();
			}

			foreach (Faction faction in factions)
			{
				faction.AdvanceTime();
			}

			HandleDeferredActions();

			foreach(Person person in people)
			{
				person.AdvanceTime();
			}

			HandleDeferredActions();

			yearsPassed++;
		}

		public Biome CalculateTileBiome(LandType landType, float rainfall, float fertility)
		{
			return Biome.CalculateBiomeType(biomes, landType, rainfall, fertility);
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

		private void AddPerson(Person person)
		{
			if(!people.Contains(person))
			{
				deferredActions.Add(() => { people.Add(person); });
			}
		}

		private void RemovePerson(Person person)
		{
			if (people.Contains(person))
			{
				deferredActions.Add(() => { people.Remove(person); });
			}
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

			BuildChunks();
			CreateBiomeMap();
			CreateVoxelBiomeMap();

			SubscribeToEvents();

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
			colorMapTexture.filterMode = FilterMode.Point;
			colorMapTexture.SetPixels(colorMap);
			colorMapTexture.Apply();
		}

		private void CreateVoxelBiomeMap()
		{
			var width = biomeMap.GetLength(0) * 2;
			var height = biomeMap.GetLength(1) * 2;
			var voxelBiomeMap = new Color[width, height];
			for (int y = 0; y < height; y+=2)
			{
				for (int x = 0; x < width; x+=2)
				{
					voxelBiomeMap[x, y] = biomeMap[x/2, y/2];
					voxelBiomeMap[x+1, y] = biomeMap[x/2, y/2];
					voxelBiomeMap[x, y+1] = biomeMap[x/2, y/2];
					voxelBiomeMap[x+1, y+1] = biomeMap[x/2, y/2];
				}
			}

			Color[] colorMap = new Color[width * height];
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					colorMap[y * width + x] = voxelBiomeMap[x, y];
				}
			}
			voxelColorMapTexture = new Texture2D(width, height);
			voxelColorMapTexture.filterMode = FilterMode.Point;
			voxelColorMapTexture.SetPixels(colorMap);
			voxelColorMapTexture.Apply();
		}

		public Texture2D CreateFactionMap()
		{
			OutputLogger.LogFormat("Regenerating faction map!", LogSource.WORLDGEN);
			var width = worldChunks.GetLength(0) * chunkSize;
			var height = worldChunks.GetLength(1) * chunkSize;
			//var width = noiseMaps[MapCategory.TERRAIN].GetLength(0);
			//var height = noiseMaps[MapCategory.TERRAIN].GetLength(1);
			var factionMap = new Color[width, height];
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					//factionMap[x, y] = new Color(0, 255, 255, 0.0f);
				}
			}

			foreach (Faction faction in factions)
			{
				foreach(Tile tile in faction.territory)
				{
					var coords = tile.GetWorldPosition();
					factionMap[coords.x, coords.y] = faction.color;
				}
			}

			Color[] colorMap = new Color[width * height];
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					colorMap[y * width + x] = factionMap[x, y];
				}
			}

			var factionMapTexture = new Texture2D(width, height);
			factionMapTexture.filterMode = FilterMode.Point;
			factionMapTexture.SetPixels(colorMap);
			factionMapTexture.Apply();

			return factionMapTexture;
		}

		private void HandleDeferredActions()
		{
			foreach(Action action in deferredActions)
			{
				action.Invoke();
			}
			deferredActions.Clear();
		}

		private void OnPersonCreated(PersonCreatedEvent simEvent)
		{
			AddPerson(simEvent.person);
		}

		private void OnPersonDeath(PersonDiedEvent simEvent)
		{
			RemovePerson(simEvent.person);
		}

		private void OnFactionCreated(FactionCreatedEvent simEvent)
		{
			deferredActions.Add(() => { factions.Add(simEvent.faction); });
		}

		private void SubscribeToEvents()
		{
			EventManager.Instance.AddEventHandler<PersonCreatedEvent>(OnPersonCreated);
			EventManager.Instance.AddEventHandler<PersonDiedEvent>(OnPersonDeath);
			EventManager.Instance.AddEventHandler<FactionCreatedEvent>(OnFactionCreated);
		}
	}
}
