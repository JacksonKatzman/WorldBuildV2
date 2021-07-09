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
		public List<City> cities;
		public Government government;
		public List<Tile> territory;
		public int influence;
		public World world;

		public int population => Population();

		public ModifiedType<float> birthRate;
		public ModifiedType<float> deathRate;
		public ModifiedType<float> foodProductionPerWorker;
		public ModifiedType<float> spoilageRate;
		public ModifiedType<float> maxFoodByLand;
		public ModifiedType<float> maxBurgeoningTension;

		public Priorities currentPriorities;

		public Military military;

		public ModifiedType<float> recruitmentRate;

		public Faction(Tile startingTile, float food, int population)
		{
			name = NameGenerator.GeneratePersonFirstName(DataManager.Instance.PrimaryNameContainer, Gender.ANY);

			territory = new List<Tile>();
			world = startingTile.world;
			territory.Add(startingTile);

			cities = new List<City>();
			cities.Add(startingTile.SpawnCity(this, food, population));
			influence = STARTING_INFLUENCE;

			military = new Military();

			SetStartingStats();
			government = new Government(this, DataManager.Instance.GetGovernmentType(influence));

			OutputLogger.LogFormatAndPause("{0} faction has been created in {1} City with government type: {2}", LogSource.FACTION, name, cities[0].name, government.governmentType.name);
		}
		public void AdvanceTime()
		{
			SetStartingStats();
			government.UpdateFactionUsingPassiveTraits(this);

			currentPriorities = GeneratePriorities();

			if(world.yearsPassed % 10 == 0)
			{
				SimAIManager.Instance.CallActionByScore(currentPriorities, this);
			}

			foreach(Tile tile in territory)
			{
				foreach(Landmark landmark in tile.landmarks)
				{
					landmark.AdvanceTime();
				}
			}

			UpdateMilitary();
		}
		public bool SpawnCityWithinRadius(Tile tile, float foodAmount, int population)
		{
			int radius = 4 + influence / 100;
			var possibleTiles = tile.GetAllTilesInRadius(radius);
			bool spawned = false;
			int attempts = 0;
			while (!spawned && attempts < 10)
			{
				var randomIndex = SimRandom.RandomRange(0, possibleTiles.Count);
				var chosenTile = possibleTiles[randomIndex];
				var tileController = world.GetFactionThatControlsTile(chosenTile);
				if (chosenTile.baseFertility > 0.5f && chosenTile.landmarks.Count == 0 && (tileController == this || tileController == null))
				{
					if(tileController == this)
					{
						cities.Add(chosenTile.SpawnCity(this, foodAmount, population));
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

		public Priorities GeneratePriorities()
		{
			int averageFactionMilitary = 0;
			float averageFactionTiles = 0;
			foreach(Faction faction in world.factions)
			{
				averageFactionTiles += faction.territory.Count;
				averageFactionMilitary += faction.military.Count;
			}

			averageFactionMilitary /= world.factions.Count;
			int militaryScore = (int)(5.0 * (averageFactionMilitary / (military.Count + 1))) + 2;

			averageFactionTiles /= world.factions.Count;
			averageFactionTiles++;

			int expansionScore = (int)(10.0f - (territory.Count / averageFactionTiles));

			return new Priorities(militaryScore, 0, 0, 5, expansionScore); 
		}

		public void UpdateMilitary()
		{
			//Handle Food Consumption
			var lostTroops = ConsumeFoodAcrossFaction(military.Count);
			military.ModifyTroopCount((int)lostTroops);
		}

		public void ModifyTroopCount(int amount)
		{
			amount = Mathf.Clamp(amount, -1 * military.Count, (int)(0.8f * population));
			RemovePopulationAcrossFaction(amount);
			military.ModifyTroopCount(amount);
		}

		public void EventRecruitTroops()
		{
			int amount = (int)(population * (currentPriorities.militaryScore / 2));
			ModifyTroopCount(amount);
			OutputLogger.LogFormat("{0} Faction has recruited {1} soldiers for it's military.", LogSource.FACTIONACTION, name, amount);
		}

		public bool ExpandTerritory()
		{
			var possibleTiles = GetBorderTiles();

			if(possibleTiles.Count == 0)
			{
				return false;
			}

			var randomIndex = SimRandom.RandomRange(0, possibleTiles.Count);
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

		public void AddCity(City city)
		{
			cities.Add(city);
		}

		public bool RemoveCity(City city)
		{
			if(cities.Contains(city))
			{
				cities.Remove(city);
				return true;
			}
			return false;
		}

		private float ConsumeFoodAcrossFaction(int amount)
		{
			Dictionary<City, float> tracker = new Dictionary<City, float>();
			foreach(City city in cities)
			{
				tracker.Add(city, city.food);
			}

			float totalfood = 0;
			foreach(float food in tracker.Values)
			{
				totalfood += food;
			}

			float totalDeficit = 0;
			foreach(var pair in tracker)
			{
				tracker[pair.Key] = pair.Value / totalfood;

				float amountConsumed = (amount * pair.Value);
				float deficit = (pair.Key.food - amountConsumed);
				if(deficit < 0)
				{
					totalDeficit += deficit;
					amountConsumed -= deficit;
				}

				pair.Key.food -= amountConsumed;
			}

			return Mathf.Clamp(totalDeficit, float.MinValue, 0);
		}

		private void RemovePopulationAcrossFaction(int amount)
		{
			if(amount >= population)
			{
				//kill faction
				return;
			}

			Dictionary<City, float> tracker = new Dictionary<City, float>();

			foreach(City city in cities)
			{
				city.population -= (city.population / population) * amount;
			}
		}

		private void RemovePercentOfPopulationAcrossFaction(float percent)
		{
			RemovePopulationAcrossFaction((int)(population * percent));
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

			recruitmentRate = new ModifiedType<float>(0);
		}

		//TODO: Define a way that factions/governments generate influence
		//TODO: Use influence and faction pressure to determine tile aquisition 
	}
}