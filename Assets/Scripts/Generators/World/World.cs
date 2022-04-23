using Game.Creatures;
using Game.Data.EventHandling;
using Game.Enums;
using Game.Factions;
using Game.Generators.Noise;
using Game.Incidents;
using Game.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.WorldGeneration
{
	public class World : ITimeSensitive, IIncidentInstigator
    {
		public Dictionary<MapCategory, float[,]> noiseMaps;
		public Color[,] biomeMap;
		public int Size => biomeMap.GetLength(0) * biomeMap.GetLength(1);

		public Texture2D heightMapTexture;
		public Texture2D colorMapTexture;
		public Texture2D voxelColorMapTexture;

		public List<Biome> biomes;
		public List<Faction> factions;
		public List<City> Cities => GetAllCities();
		private List<War> wars;

		NoiseSettings noiseSettings;
        public Chunk[,] worldChunks;
		public int chunkSize;
		public int yearsPassed;

		public List<ICreature> creatures;
		public List<Person> People => GetPeople();

		public List<OngoingEvent> ongoingEvents;
		private List<Action> deferredActions;

		public World(float[,] noiseMap, Texture2D texture, NoiseSettings settings, int chunkSize, List<Biome> biomes)
		{
			noiseMaps = new Dictionary<MapCategory, float[,]>();
			factions = new List<Faction>();
			creatures = new List<ICreature>();
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

		private List<Person> GetPeople()
		{
			var people = new List<Person>();
			foreach(var creature in creatures)
			{
				if(creature is Person person)
				{
					people.Add(person);
				}
			}
			return people;
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

			//SimAIManager.Instance.CallWorldEvent(this);

			GenerateNewNoiseMap(MapCategory.RAINFALL);

			foreach(Chunk chunk in worldChunks)
			{
				chunk.AdvanceTime();
			}

			SimulationManager.Instance.timer.Tic();
			foreach (Faction faction in factions)
			{
				faction.currentPriorities = faction.GeneratePriorities();
			}

			foreach (Faction faction in factions)
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
			foreach (Person person in creatures)
			{
				person.AdvanceTime();
			}
			SimulationManager.Instance.timer.Toc("People");

			DeathEvent();

			HandleDeferredActions();

			yearsPassed++;
		}

		public void DeathEvent()
		{
			var tags = new List<IIncidentTag> { new InstigatorTag(typeof(World)), new WorldTag(new List<WorldTagType> { WorldTagType.DEATH }), new SpecialCaseTag(SpecialCaseTagType.END_OF_TURN) };
			var context = new IncidentContext(this, tags);
			IncidentService.Instance.PerformIncident(context);
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

		public bool AttemptWar(Faction aggressor, Faction defender)
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
			if (wars.Contains(war))
			{
				OutputLogger.LogFormatAndPause("A war between has ended.", LogSource.IMPORTANT);
				deferredActions.Add(() => { wars.Remove(war); });
			}
		}

		public List<Person> GetPeopleFromFaction(Faction faction)
		{
			var people = creatures.Where(x => x is Person) as List<Person>;
			return people.Where(x => x.faction == faction).ToList();
		}

		private List<City> GetAllCities()
		{
			var cities = new List<City>();
			foreach(var faction in factions)
			{
				cities.AddRange(faction.cities);
			}

			return cities;
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

		private void AddCreature(ICreature creature)
		{
			if(!creatures.Contains(creature))
			{
				deferredActions.Add(() => { creatures.Add(creature); });
			}
		}

		private void RemoveCreature(ICreature creature)
		{
			if (creatures.Contains(creature))
			{
				deferredActions.Add(() => { creatures.Remove(creature); });
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

		public void BuildRoads()
		{
			var tileMap = new Tile[worldChunks.GetLength(0) * chunkSize, worldChunks.GetLength(1) * chunkSize];
			foreach (var chunk in worldChunks)
			{
				foreach (var tile in chunk.chunkTiles)
				{
					var pos = tile.GetWorldPosition();
					tileMap[pos.x, pos.y] = tile;
				}
			}

			//roads to capital cities
			if (factions.Count > 1)
			{
				float DetermineAStarCost(Tile from, Tile to)
				{
					if(to.roadDirections.Count > 0)
					{
						return 0;
					}
					var toHeight = to.chunk.SampleNoiseMap(Enums.MapCategory.TERRAIN, to.coords);
					var fromHeight = from.chunk.SampleNoiseMap(Enums.MapCategory.TERRAIN, from.coords);
					var riverCost = 0;
					if(to.biome.landType == LandType.RIVER)
					{
						if(to.riverDirections.Count == 2 &&
						((to.riverDirections.Contains(Direction.SOUTH) && to.riverDirections.Contains(Direction.NORTH)) || (to.riverDirections.Contains(Direction.WEST) && to.riverDirections.Contains(Direction.EAST))) &&
						from.biome.landType != LandType.RIVER)
						{
							riverCost = 5;
						}
						else
						{
							riverCost = 100000;
						}
					}
					var oceanCost = to.biome.landType == LandType.OCEAN ? 100000 : 0;
					return (1.0f * (1.0f + Mathf.Abs(toHeight - fromHeight))) + riverCost + oceanCost;
				}

				var remainingFactions = new List<Faction>();
				remainingFactions.AddRange(factions);
				var current = SimRandom.RandomEntryFromList(remainingFactions);
				do
				{
					remainingFactions.Remove(current);
					remainingFactions.OrderBy(x => Tile.GetDistanceBetweenTiles(current.cities[0].tile, x.cities[0].tile));
					var closest = remainingFactions[0];

					var aStarPath = AStarPathfinder.AStarBestPath(current.cities[0].tile, closest.cities[0].tile, tileMap, DetermineAStarCost);

					for (int a = 1; a < aStarPath.Count - 1; a++)
					{
						var currentTile = aStarPath[a];
						if (currentTile.biome.landType != LandType.OCEAN)
						{
							currentTile.AddRoadDirection(currentTile.DetermineAdjacentRelativeDirection(aStarPath[a - 1]));
							currentTile.AddRoadDirection(currentTile.DetermineAdjacentRelativeDirection(aStarPath[a + 1]));
						}
					}

					current = closest;
				}
				while (remainingFactions.Count > 1);
			}
		}

		private void BuildRivers()
		{
			SimulationManager.Instance.timer.Tic();

			var unsortedTiles = new List<Tile>();
			var tileMap = new Tile[worldChunks.GetLength(0) * chunkSize, worldChunks.GetLength(1) * chunkSize];
			foreach(var chunk in worldChunks)
			{
				foreach(var tile in chunk.chunkTiles)
				{
					unsortedTiles.Add(tile);
					var pos = tile.GetWorldPosition();
					tileMap[pos.x, pos.y] = tile;
				}
			}

			var mountainTiles = unsortedTiles.Where(x => x.biome.landType == LandType.MOUNTAINS).ToList();
			var oceanTiles = unsortedTiles.Where(x => x.biome.landType == LandType.OCEAN).ToList();

			float DetermineAStarCost(Tile from, Tile to)
			{
				var toHeight = to.chunk.SampleNoiseMap(Enums.MapCategory.TERRAIN, to.coords);
				var fromHeight = from.chunk.SampleNoiseMap(Enums.MapCategory.TERRAIN, from.coords);
				return 1.0f * (1.0f + Mathf.Abs(toHeight - fromHeight));
			}

			for (int i = 0; i < 12; i++)
			{
				if(mountainTiles.Count == 0 || oceanTiles.Count == 0)
				{
					break;
				}

				var randomMountainTile = SimRandom.RandomEntryFromList(mountainTiles);
				var targetOceanTile = oceanTiles[0];
				for (int a = 1; a < oceanTiles.Count; a++)
				{
					targetOceanTile = Tile.GetDistanceBetweenTiles(randomMountainTile, oceanTiles[a]) < Tile.GetDistanceBetweenTiles(randomMountainTile, targetOceanTile) ? oceanTiles[a] : targetOceanTile;
				}

				var aStarPath = AStarPathfinder.AStarBestPath(randomMountainTile, targetOceanTile, tileMap, DetermineAStarCost);

				for(int a = 1; a < aStarPath.Count - 1; a++)
				{
					var currentTile = aStarPath[a];
					if (currentTile.biome.landType != LandType.OCEAN)
					{
						currentTile.biome = Biome.GetRandomBiomeByLandType(biomes, LandType.RIVER);
						currentTile.AddRiverDirection(currentTile.DetermineAdjacentRelativeDirection(aStarPath[a - 1]));
						currentTile.AddRiverDirection(currentTile.DetermineAdjacentRelativeDirection(aStarPath[a + 1]));
					}
				}

				var nearbyMountains = mountainTiles.Where(x => Tile.GetDistanceBetweenTiles(x, randomMountainTile) < 3).ToList();
				foreach(var tile in nearbyMountains)
				{
					if(mountainTiles.Contains(tile))
					{
						mountainTiles.Remove(tile);
					}
				}
				oceanTiles.Remove(targetOceanTile);
			}

			SimulationManager.Instance.timer.Toc("Build Rivers");
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
			BuildRivers();
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

		private void OnCreatureCreated(CreatureCreatedEvent simEvent)
		{
			AddCreature(simEvent.creature);
		}

		private void OnCreatureDeath(CreatureDiedEvent simEvent)
		{
			RemoveCreature(simEvent.creature);
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

				foreach(var war in wars)
				{
					if(war.victories.Keys.Contains(simEvent.faction))
					{
						ResolveWar(war);
					}
				}
			}
		}

		private void SubscribeToEvents()
		{
			EventManager.Instance.AddEventHandler<CreatureCreatedEvent>(OnCreatureCreated);
			EventManager.Instance.AddEventHandler<CreatureDiedEvent>(OnCreatureDeath);
			EventManager.Instance.AddEventHandler<FactionCreatedEvent>(OnFactionCreated);
			EventManager.Instance.AddEventHandler<FactionDestroyedEvent>(OnFactionDestroyed);
		}
	}
}
