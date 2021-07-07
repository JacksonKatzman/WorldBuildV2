using Game.Enums;
using Game.WorldGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Factions
{
	public class Faction : ITimeSensitive
	{
		private static int STARTING_INFLUENCE = 10;
		private static float AVERAGE_BIRTH_RATE = 0.0205f;
		private static float AVERAGE_DEATH_RATE = 0.0078f;
		private static float AVERAGE_FOOD_PRODUCTION = 5.0f;
		private static float AVERAGE_SPOILAGE_RATE = 0.1f;
		private static float MAX_FOOD_BY_LAND = 10000.0f;
		private static float MAX_BURGEONING_TENSION = MAX_FOOD_BY_LAND / 10;

		public string name;
		public City capitalCity;
		public Government government;
		public List<Tile> territory;
		public int influence;
		public World world;

		public int population => Population();
		public int militarySize;

		public ModifiedType<float> birthRate;
		public ModifiedType<float> deathRate;
		public ModifiedType<float> foodProductionPerWorker;
		public ModifiedType<float> spoilageRate;
		public ModifiedType<float> maxFoodByLand;
		public ModifiedType<float> maxBurgeoningTension;

		public Faction(Tile startingTile, float food, int population)
		{
			name = NameGenerator.GeneratePersonFirstName(DataManager.Instance.PrimaryNameContainer, Gender.NEITHER);

			territory = new List<Tile>();
			world = startingTile.world;
			territory.Add(startingTile);
			capitalCity = startingTile.SpawnCity(this, food, population);
			influence = STARTING_INFLUENCE;

			SetStartingStats();
			government = new Government(DataManager.Instance.GetGovernmentType(influence));

			OutputLogger.LogFormatAndPause("{0} faction has been created in {1} City with government type: {2}", LogSource.FACTION, name, capitalCity.name, government.governmentType.name);
		}
		public void AdvanceTime()
		{
			SetStartingStats();
			government.UpdateFactionUsingPassiveTraits(this);

			if(world.yearsPassed % 10 == 0)
			{
				var score = GenerateActionScore();
				EventManager.Instance.CallActionByScore(score, this);
			}

			foreach(Tile tile in territory)
			{
				foreach(Landmark landmark in tile.landmarks)
				{
					landmark.AdvanceTime();
				}
			}
		}
		public bool SpawnCityWithinRadius(Tile tile, float foodAmount, int population)
		{
			int radius = 4 + influence / 100;
			var possibleTiles = tile.GetAllTilesInRadius(radius);
			bool spawned = false;
			int attempts = 0;
			while (!spawned && attempts < 10)
			{
				var randomIndex = WorldHandler.Instance.RandomRange(0, possibleTiles.Count);
				var chosenTile = possibleTiles[randomIndex];
				var tileController = world.GetFactionThatControlsTile(chosenTile);
				if (chosenTile.baseFertility > 0.5f && chosenTile.landmarks.Count == 0 && (tileController == this || tileController == null))
				{
					if(tileController == this)
					{
						chosenTile.SpawnCity(this, foodAmount, population);
					}
					else
					{
						world.CreateNewFaction(chosenTile, foodAmount, population);
					}
					spawned = true;
					OutputLogger.LogFormatAndPause("Spawned city in chunk ({0},{1}) in tile ({2},{3})).",
						LogSource.WORLDGEN, chosenTile.chunk.coords.x, chosenTile.chunk.coords.y, chosenTile.coords.x, chosenTile.coords.y);
					OutputLogger.LogFormat("World Coords are: {0},{1}.", LogSource.WORLDGEN, chosenTile.GetWorldPosition(), tile.GetWorldPosition());
				}
				attempts++;
			}
			return spawned;
		}

		public ActionScore GenerateActionScore()
		{
			int militaryScore = 10 - (10 * militarySize) / population;

			float averageFactionTiles = 0;
			foreach(Faction faction in world.factions)
			{
				averageFactionTiles += faction.territory.Count;
			}
			averageFactionTiles /= world.factions.Count;
			averageFactionTiles++;

			int expansionScore = (int)(10.0f - (territory.Count / averageFactionTiles));

			return new ActionScore(militaryScore, 0, 0, 5, expansionScore); 
		}

		public bool ExpandTerritory()
		{
			var possibleTiles = GetBorderTiles();

			if(possibleTiles.Count == 0)
			{
				return false;
			}

			var randomIndex = WorldHandler.Instance.RandomRange(0, possibleTiles.Count);
			var chosenTile = possibleTiles[randomIndex];
			territory.Add(chosenTile);
			OutputLogger.LogFormat("{0} Faction expanded it's borders to include the tile at {1}.", Game.Enums.LogSource.FACTIONACTION, name, chosenTile.GetWorldPosition());
			return true;
		}

		private List<Tile> GetBorderTiles()
		{
			var possibleTiles = new List<Tile>();
			foreach(Tile territoryTile in territory)
			{
				var adjacentTiles = territoryTile.GetAllTilesInRadius(2);
				foreach(Tile adjacentTile in adjacentTiles)
				{
					var tileController = world.GetFactionThatControlsTile(adjacentTile);
					if (tileController == null && !possibleTiles.Contains(adjacentTile))
					{
						possibleTiles.Add(adjacentTile);
					}
				}
			}
			return possibleTiles;
		}

		private int Population()
		{
			int count = 0;
			foreach(Tile tile in territory)
			{
				foreach(Landmark landmark in tile.landmarks)
				{
					if(landmark is City city)
					{
						count += city.population;
					}
				}
			}
			return count;
		}

		private void SetStartingStats()
		{
			birthRate = new ModifiedType<float>(AVERAGE_BIRTH_RATE);
			deathRate = new ModifiedType<float>(AVERAGE_DEATH_RATE);
			foodProductionPerWorker = new ModifiedType<float>(AVERAGE_FOOD_PRODUCTION);
			spoilageRate = new ModifiedType<float>(AVERAGE_SPOILAGE_RATE);
			maxFoodByLand = new ModifiedType<float>(MAX_FOOD_BY_LAND);
			maxBurgeoningTension = new ModifiedType<float>(MAX_BURGEONING_TENSION);
		}

		//TODO: Create basic government types and spawn governments with those types
		//TODO: Define a way that factions/governments generate influence
		//TODO: Use influence and faction pressure to determine tile aquisition 
	}
}