using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Generators.Noise;
using Game.Enums;
using Game.Factions;
using System.Linq;
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
		public List<FactionSimulator> factions;
		private List<War> wars;

		NoiseSettings noiseSettings;
        private Chunk[,] worldChunks;
		public int chunkSize;
		public int yearsPassed;

		private List<Person> people;

		public List<OngoingEvent> ongoingEvents;
		private List<Action> deferredActions;

		public World(float[,] noiseMap, Texture2D texture, NoiseSettings settings, int chunkSize, List<Biome> biomes)
		{
			noiseMaps = new Dictionary<MapCategory, float[,]>();
			factions = new List<FactionSimulator>();
			people = new List<Person>();
			wars = new List<War>();
			ongoingEvents = new List<OngoingEvent>();
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
			OutputLogger.LogFormat("Beginning World Advance Time!", LogSource.MAIN);

			HandleDeferredActions();

			HandleOngoingEvents();

			SimAIManager.Instance.CallWorldEvent(this);

			GenerateNewNoiseMap(MapCategory.RAINFALL);

			foreach(Chunk chunk in worldChunks)
			{
				chunk.AdvanceTime();
			}

			SimulationManager.Instance.timer.Tic();
			foreach (FactionSimulator faction in factions)
			{
				faction.currentPriorities = faction.GeneratePriorities();
			}

			foreach (FactionSimulator faction in factions)
			{
				faction.AdvanceTime();
			}
			SimulationManager.Instance.timer.Toc("Faction");

			SimulationManager.Instance.timer.Tic();
			foreach (War war in wars)
			{
				war.AdvanceTime();
			}
			SimulationManager.Instance.timer.Toc("WAR");
			HandleDeferredActions();

			SimulationManager.Instance.timer.Tic();
			foreach (Person person in people)
			{
				person.AdvanceTime();
			}
			SimulationManager.Instance.timer.Toc("People");
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

		public Tile GetRandomTile()
		{
			var randomXIndex = SimRandom.RandomRange(0, noiseMaps[Enums.MapCategory.TERRAIN].GetLength(0));
			var randomYIndex = SimRandom.RandomRange(0, noiseMaps[Enums.MapCategory.TERRAIN].GetLength(1));
			return GetTileAtWorldPosition(new Vector2Int(randomXIndex, randomYIndex));
		}

		public FactionSimulator GetFactionThatControlsTile(Tile tile)
		{
			FactionSimulator controllingFaction = null;
			foreach(FactionSimulator faction in factions)
			{
				if(faction.territory.Contains(tile))
				{
					controllingFaction = faction;
					break;
				}
			}

			return controllingFaction;
		}

		public bool AttemptWar(FactionSimulator aggressor, FactionSimulator defender)
		{
			var exisitingWar = false;
			foreach(War war in wars)
			{
				if(war.originalAggressor == aggressor && war.originalDefender == defender)
				{
					exisitingWar = true;
				}
				else if (war.originalAggressor == defender && war.originalDefender == aggressor)
				{
					exisitingWar = true;
				}
			}

			if (!exisitingWar)
			{
				wars.Add(new War(this, aggressor, defender));
				OutputLogger.LogFormatAndPause("A war has begun between {0} Faction and {1} Faction!", LogSource.IMPORTANT, aggressor.Name, defender.Name);
			}

			//will add a chance to avoid this with diplomacy later
			return !exisitingWar;
		}

		public void ResolveWar(War war)
		{
			OutputLogger.LogFormatAndPause("A war between has ended.", LogSource.IMPORTANT);
			deferredActions.Add(() => { wars.Remove(war); });
		}

		public List<Person> GetPeopleFromFaction(FactionSimulator faction)
		{
			var query =
				from person in people
				where person.faction == faction
				select person;
			return query.ToList();
		}

		private void HandleOngoingEvents()
		{
			foreach(var ongoingEvent in ongoingEvents)
			{
				ongoingEvent.eventAction.Invoke();
				ongoingEvent.duration--;
			}

			ongoingEvents.RemoveAll(oe => oe.duration <= 0);
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
			var factionMap = new Color[width, height];
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					//factionMap[x, y] = new Color(0, 255, 255, 0.0f);
				}
			}

			foreach (FactionSimulator faction in factions)
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

		public void HandleCleanup()
		{
			foreach(Chunk chunk in worldChunks)
			{
				foreach(Tile tile in chunk.chunkTiles)
				{
					var controller = tile.controller;
					if(controller != null)
					{
						var distanceToCity = controller.GetDistanceToNearestCity(tile);
						if(distanceToCity > controller.territory.Count/5)
						{
							controller.territory.Remove(tile);
							tile.controller = null;
						}
					}
				}
			}
		}

		public void HandleDeferredActions()
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
			if (!factions.Contains(simEvent.faction))
			{
				deferredActions.Add(() => { factions.Add(simEvent.faction); });
			}
		}

		private void OnFactionDestroyed(FactionDestroyedEvent simEvent)
		{
			if (factions.Contains(simEvent.faction))
			{
				deferredActions.Add(() => { factions.Remove(simEvent.faction); });
			}
		}

		private void SubscribeToEvents()
		{
			EventManager.Instance.AddEventHandler<PersonCreatedEvent>(OnPersonCreated);
			EventManager.Instance.AddEventHandler<PersonDiedEvent>(OnPersonDeath);
			EventManager.Instance.AddEventHandler<FactionCreatedEvent>(OnFactionCreated);
			EventManager.Instance.AddEventHandler<FactionDestroyedEvent>(OnFactionDestroyed);
		}
	}
}
