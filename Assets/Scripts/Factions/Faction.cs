using Game.Enums;
using Game.WorldGeneration;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Generators;
using Game.Data.EventHandling;

namespace Game.Factions
{
	public class Faction : ITimeSensitive
	{
		private static int STARTING_INFLUENCE = 10;
		private static float AVERAGE_BIRTH_RATE = 0.0165f;
		private static float AVERAGE_DEATH_RATE = 0.0078f;
		private static float AVERAGE_FOOD_PRODUCTION = 5.0f;
		private static float AVERAGE_SPOILAGE_RATE = 0.1f;
		private static float MAX_FOOD_BY_LAND = 10000.0f;
		private static float MAX_BURGEONING_TENSION = MAX_FOOD_BY_LAND / 10;
		private static float BASE_REBELLION_CHANCE = 0.05f;

		public string name;
		public Color color;
		public List<City> cities;
		public Government government;
		public List<Tile> territory;
		public List<Tile> borderTiles;
		public int influence;
		public World world;

		public int actionsRemaining;
		private float foodProducedThisTurn;

		public int population => Population();

		public ModifiedType<int> actionsPerTurn;

		public ModifiedType<float> birthRate;
		public ModifiedType<float> deathRate;
		public ModifiedType<float> foodProductionPerWorker;
		public ModifiedType<float> spoilageRate;
		public ModifiedType<float> maxFoodByLand;
		public ModifiedType<float> maxBurgeoningTension;
		public ModifiedType<float> rebellionChance;

		public Priorities currentPriorities;

		public Military military;

		public ModifiedType<float> recruitmentRate;

		private int turnsSinceLastExpansion = 0;

		private List<Action> deferredActions;

		public Faction(Tile startingTile, float food, int population)
		{
			name = NameGenerator.GeneratePersonFirstName(DataManager.Instance.PrimaryNameContainer, Gender.ANY);
			var r = SimRandom.RandomFloat01();
			var g = SimRandom.RandomFloat01();
			var b = SimRandom.RandomFloat01();
			color = new Color(r, g, b, 0.4f * 255);

			territory = new List<Tile>();
			world = startingTile.world;

			cities = new List<City>();
			cities.Add(LandmarkGenerator.SpawnCity(startingTile, this, food, population));
			influence = STARTING_INFLUENCE;

			military = new Military();

			SetStartingStats();
			government = new Government(this, DataManager.Instance.GetGovernmentType(influence));

			deferredActions = new List<Action>();

			SubscribeToEvents();

			OutputLogger.LogFormatAndPause("{0} faction has been created in {1} City with government type: {2}", LogSource.FACTION, name, cities[0].name, government.governmentType.name);
		}
		public void AdvanceTime()
		{
			OutputLogger.LogFormat("Beginning {0} Faction Advance Time!", LogSource.MAIN, name);

			HandleDeferredActions();

			ResetTurnSpecificValues();

			government.UpdateFactionUsingPassiveTraits(this);

			foreach (Tile tile in territory)
			{
				foreach(Landmark landmark in tile.landmarks)
				{
					landmark.AdvanceTime();
				}
			}

			UpdateMilitary();

			SimAIManager.Instance.CallActionByScores(currentPriorities, this);

			HandleDeferredActions();

			turnsSinceLastExpansion++;
		}
		public bool SpawnCityWithinRadius(Tile tile, float foodAmount, int population)
		{
			int maxRadius = 5 + influence / 100;
			int minRadius = 3;
			var possibleTiles = tile.GetAllTilesInRing(maxRadius, minRadius);
			bool spawned = false;
			int attempts = 0;
			while (!spawned && attempts < 10)
			{
				var randomIndex = SimRandom.RandomRange(0, possibleTiles.Count);
				var chosenTile = possibleTiles[randomIndex];
				var tileController = world.GetFactionThatControlsTile(chosenTile);
				if (LandmarkGenerator.IsSuitableCityLocation(chosenTile, 0.5f, 0.2f, this))
				{
					var keep = SimRandom.RandomFloat01();
					if(keep > rebellionChance.modified)
					{
						deferredActions.Add(() => { cities.Add(LandmarkGenerator.SpawnCity(chosenTile, this, foodAmount, population)); });
					}
					else
					{
						FactionGenerator.SpawnFaction(chosenTile, foodAmount, population);
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
			int totalFactionTiles = 0;
			foreach(Faction faction in world.factions)
			{
				totalFactionTiles += faction.territory.Count;
			}

			var averageFactionTiles = totalFactionTiles / world.factions.Count;
			averageFactionTiles++;

			var expansionExhaustion = Mathf.Clamp(turnsSinceLastExpansion, 0, 10) / 10.0f;
			var averageOverTerritoryScore = (10.0f * averageFactionTiles / territory.Count);
			var citiesOverTerritoryScore = (3.0f * cities.Count / territory.Count);
			var claimedOverSizeScore = (10.0f * totalFactionTiles / world.Size);

			int expansionScore = (int)((averageOverTerritoryScore * citiesOverTerritoryScore * expansionExhaustion) - claimedOverSizeScore);

			var averageLeaderPriorities = new Priorities();
			foreach(LeadershipStructureNode node in government.leadershipStructure[0].tier)
			{
				averageLeaderPriorities = averageLeaderPriorities + node.occupant.priorities;
			}
			averageLeaderPriorities = averageLeaderPriorities / government.leadershipStructure[0].tier.Count;

			var factionPriorities = new Priorities(0, 0, 0, 3, expansionScore, 0);
			var totalPriorities = factionPriorities + averageLeaderPriorities;

			return totalPriorities; 
		}

		public void UpdateMilitary()
		{
			//Handle Food Consumption
			var lostTroops = ConsumeFoodAcrossFaction(military.Count);
			if (lostTroops > 0)
			{
				ModifyTroopCount((int)lostTroops);
			}

			var foodSurplus = (foodProducedThisTurn - population) - military.Count;
			if(foodSurplus > population * 1.2f)
			{
				//can probably sustain more troops safely
				int averageFactionMilitary = 0;
				foreach (Faction faction in world.factions)
				{
					averageFactionMilitary += faction.military.Count;
				}

				averageFactionMilitary /= world.factions.Count;
				averageFactionMilitary++;
				int militaryScore = (int)(5.0 * ((averageFactionMilitary * 1.1f) / (military.Count + 1.0f)));
				var maxRecruitablePop = population * 0.2f;
				//var recruitmentMulitplier = 10 - (10 * (military.Count / maxRecruitablePop));
				//recruitmentMulitplier = Mathf.Clamp(recruitmentMulitplier, 0.0f, 10.0f);

				var priorityImpact = currentPriorities.priorities[PriorityType.MILITARY] / 20;
				var securityImpact = Mathf.Clamp(militaryScore, 0.0f, 5.0f) / 10;
				var combinedImpact = (priorityImpact + securityImpact) / 2.0f;
				var maxTroopsBySurplus = foodSurplus * combinedImpact;
				var maxTroopsByPop = (maxRecruitablePop - military.Count) * combinedImpact;

				ModifyTroopCount((int)Mathf.Min(maxTroopsByPop, maxTroopsBySurplus));
				actionsRemaining--;
			}
			else if(foodSurplus > 0)
			{
				//we could hypothetically have more troops but probably shouldnt

			}
			else
			{
				//need to swords to plowshares
				var percentToConvert = 1.0f -((10 - (10 / (currentPriorities.priorities[PriorityType.MILITARY] + 1))) / 10);
				ModifyTroopCount((int)(foodSurplus * percentToConvert));
				actionsRemaining--;
			}
		}

		public void ModifyTroopCount(int amount)
		{
			amount = Mathf.Clamp(amount, -1 * military.Count, (int)(0.8f * population));
			RemovePopulationAcrossFaction(amount);
			military.ModifyTroopCount(amount);
			OutputLogger.LogFormat("{0} Faction's troop count has shifted by {1}.", LogSource.FACTIONACTION, name, amount);
		}

		public void EventRecruitTroops()
		{
			int amount = (int)(population * (currentPriorities.priorities[PriorityType.MILITARY] / 100.0f));
			ModifyTroopCount(amount);
		}

		public bool ExpandTerritory()
		{
			var possibleTiles = borderTiles;

			if(possibleTiles.Count == 0)
			{
				return false;
			}

			var randomIndex = SimRandom.RandomRange(0, possibleTiles.Count);
			var chosenTile = possibleTiles[randomIndex];
			territory.Add(chosenTile);
			turnsSinceLastExpansion = 0;
			OutputLogger.LogFormat("{0} Faction expanded it's borders to include the tile at {1}.", Game.Enums.LogSource.FACTIONACTION, name, chosenTile.GetWorldPosition());
			return true;
		}

		private List<Tile> GetBorderTiles()
		{
			var possibleTiles = new List<Tile>();
			foreach(Tile territoryTile in territory)
			{
				var adjacentTiles = territoryTile.GetDirectlyAdjacentTiles();
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

		public void RemoveLandmark(Tile tile, Landmark landmark)
		{
			deferredActions.Add(() => { tile.landmarks.Remove(landmark); });
			if(landmark is City city)
			{
				deferredActions.Add(() => { RemoveCity(city); });
			}
		}

		public void ReportFoodProduced(float food)
		{
			foodProducedThisTurn += food;
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
				var ratio = pair.Value / totalfood;

				float amountConsumed = (amount * ratio);
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
			actionsPerTurn = new ModifiedType<int>(1);

			birthRate = new ModifiedType<float>(AVERAGE_BIRTH_RATE);
			deathRate = new ModifiedType<float>(AVERAGE_DEATH_RATE);
			foodProductionPerWorker = new ModifiedType<float>(AVERAGE_FOOD_PRODUCTION);
			spoilageRate = new ModifiedType<float>(AVERAGE_SPOILAGE_RATE);
			maxFoodByLand = new ModifiedType<float>(MAX_FOOD_BY_LAND);
			maxBurgeoningTension = new ModifiedType<float>(MAX_BURGEONING_TENSION);
			rebellionChance = new ModifiedType<float>(BASE_REBELLION_CHANCE);

			recruitmentRate = new ModifiedType<float>(0);
		}

		private void HandleDeferredActions()
		{
			foreach (Action action in deferredActions)
			{
				action.Invoke();
			}
			deferredActions.Clear();
		}

		private void ResetTurnSpecificValues()
		{
			SetStartingStats();

			actionsRemaining = actionsPerTurn.modified;
			foodProducedThisTurn = 0;
			borderTiles = GetBorderTiles();
		}

		private void CheckForDestruction()
		{
			if(cities.Count <= 0)
			{
				FactionGenerator.DestroyFaction(this);
			}
		}

		private void OnRecievedCityDestroyed(CityDestroyedEvent simEvent)
		{
			if(cities.Contains(simEvent.city))
			{
				RemoveLandmark(simEvent.city.tile, simEvent.city);
			}
		}

		private void SubscribeToEvents()
		{
			EventManager.Instance.AddEventHandler<CityDestroyedEvent>(OnRecievedCityDestroyed);
		}

		//TODO: Define a way that factions/governments generate influence
		//TODO: Use influence and faction pressure to determine tile aquisition 
	}
}