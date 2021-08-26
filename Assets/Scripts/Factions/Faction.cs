using Game.Enums;
using Game.WorldGeneration;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Generators;
using Game.Data.EventHandling;
using Game.Math;
using Game.Data.EventHandling.EventRecording;

namespace Game.Factions
{
	public class Faction : ITimeSensitive, IRecordable
	{
		private static int STARTING_INFLUENCE = 10;
		private static float AVERAGE_BIRTH_RATE = 0.0165f;
		private static float AVERAGE_DEATH_RATE = 0.0078f;
		private static float AVERAGE_FOOD_PRODUCTION = 5.0f;
		private static float AVERAGE_SPOILAGE_RATE = 0.1f;
		private static float MAX_FOOD_BY_LAND = 10000.0f;
		private static float MAX_BURGEONING_TENSION = MAX_FOOD_BY_LAND / 10;
		private static float BASE_REBELLION_CHANCE = 0.05f;
		private static float BASE_FACTION_PRESSURE_THRESHOLD = 100.0f;

		private string name;
		public string Name => name;
		public Color color;
		public List<City> cities;
		public Government government;
		public List<Tile> territory;
		private Polygon maxBorderPolygon;
		private List<Tile> nextExpansions;

		public List<Person> People => world.GetPeopleFromFaction(this);

		public List<Tile> borderTiles;
		public Dictionary<Faction, float> factionTensions;
		public int influence;
		public World world;

		public int actionsRemaining;
		private float foodProducedThisTurn;

		public int population => Population();

		public ModifiedInt actionsPerTurn;

		public ModifiedFloat birthRate;
		public ModifiedFloat deathRate;
		public ModifiedFloat foodProductionPerWorker;
		public ModifiedFloat spoilageRate;
		public ModifiedFloat maxFoodByLand;
		public ModifiedFloat maxBurgeoningTension;
		public ModifiedFloat rebellionChance;
		public ModifiedFloat factionPressureModifier;
		public ModifiedFloat factionPressureThreshold;

		public Priorities currentPriorities;

		public ModifiedFloat recruitmentRate;

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
			nextExpansions = new List<Tile>();
			world = startingTile.world;

			cities = new List<City>();
			AddCity(LandmarkGenerator.SpawnCity(startingTile, this, food, population));
			influence = STARTING_INFLUENCE;

			SetStartingStats();
			government = new Government(this);
			government.UpdateFactionUsingPassiveTraits(this);

			factionTensions = new Dictionary<Faction, float>();

			deferredActions = new List<Action>();

			SubscribeToEvents();

			OutputLogger.LogFormat("{0} faction has been created in {1} City.", LogSource.FACTION, name, cities[0].Name);
		}
		public void AdvanceTime()
		{
			OutputLogger.LogFormat("Beginning {0} Faction Advance Time!", LogSource.MAIN, name);

			HandleDeferredActions();

			ResetTurnSpecificValues();

			government.HandleUpgrades(population);

			ExpandTerritory();

			//UpdateMilitary();

			CalculateFactionTension();

			SimAIManager.Instance.CallFactionActionByScores(currentPriorities, this);

			UpdateStats();

			HandleDeferredActions();

			CheckForDestruction();

			turnsSinceLastExpansion++;
		}
		public bool SpawnCityWithinRadius(Tile tile, float foodAmount, int population)
		{
			if(nextExpansions.Count > 0 || cities.Count >= 5 + territory.Count/40)
			{
				return false;
			}
			int maxRadius = 8 + influence / 100;
			int minRadius = 6;
			var possibleTiles = tile.GetAllTilesInRing(maxRadius, minRadius);
			bool spawned = false;
			int attempts = 0;
			while (!spawned && attempts < 10)
			{
				var randomIndex = SimRandom.RandomRange(0, possibleTiles.Count);
				var chosenTile = possibleTiles[randomIndex];
				var tileController = chosenTile.controller;
				if (LandmarkGenerator.IsSuitableCityLocation(chosenTile, 0.5f, 0.2f, this))
				{
					var keep = SimRandom.RandomFloat01();
					if(keep > rebellionChance.Modified)
					{
						deferredActions.Add(() => { AddCity(LandmarkGenerator.SpawnCity(chosenTile, this, foodAmount, population)); });
					}
					else
					{
						FactionGenerator.SpawnFaction(chosenTile, foodAmount, population);
					}
					spawned = true;
					OutputLogger.LogFormat("Spawned city in chunk ({0},{1}) in tile ({2},{3})).",
						LogSource.WORLDGEN, chosenTile.chunk.coords.x, chosenTile.chunk.coords.y, chosenTile.coords.x, chosenTile.coords.y);
					OutputLogger.LogFormat("World Coords are: {0},{1}.", LogSource.WORLDGEN, chosenTile.GetWorldPosition(), tile.GetWorldPosition());
				}
				attempts++;
			}
			return spawned;
		}

		public Priorities GeneratePriorities()
		{
			var averageLeaderPriorities = new Priorities();
			foreach(LeadershipStructureNode node in government.leadershipStructure[0].tier)
			{
				//averageLeaderPriorities = averageLeaderPriorities + node.occupant.priorities;
			}
			averageLeaderPriorities = averageLeaderPriorities / government.leadershipStructure[0].tier.Count;

			var factionPriorities = new Priorities(0, 0, 0, 3, 0);
			var totalPriorities = factionPriorities + averageLeaderPriorities;

			return totalPriorities; 
		}

		/*
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
		*/
		/*
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

		public void RecruitPercentOfPopulation(float percent)
		{
			percent /= 100.0f;
			int amount = (int)(population * percent);
			ModifyTroopCount(amount);
		}
		*/

		private void ExpandTerritory()
		{
			var expansionExhaustion = Mathf.Clamp(turnsSinceLastExpansion, 0, 10) / 10.0f;
			var emptyBorderTiles = GetEmptyBorderTiles();

			if (nextExpansions.Count > 0)
			{
				if (expansionExhaustion * 2 >= 1.0f)
				{
					nextExpansions[0].ChangeControl(this);
					nextExpansions.Remove(nextExpansions[0]);
					turnsSinceLastExpansion = 0;
				}
			}
			else
			{
				if(expansionExhaustion >= 1.0f && emptyBorderTiles.Count > 0)
				{
					var randomIndex = SimRandom.RandomRange(0, emptyBorderTiles.Count);
					emptyBorderTiles[randomIndex].ChangeControl(this);
					turnsSinceLastExpansion = 0;
				}
			}
		}

		private List<Tile> GetEmptyBorderTiles()
		{
			var possibleTiles = new List<Tile>();
			foreach(Tile territoryTile in territory)
			{
				var adjacentTiles = territoryTile.GetDirectlyAdjacentTiles();
				foreach(Tile adjacentTile in adjacentTiles)
				{
					var tileController = adjacentTile.controller;
					if (tileController == null && !possibleTiles.Contains(adjacentTile))
					{
						possibleTiles.Add(adjacentTile);
					}
				}
			}
			return possibleTiles;
		}

		public List<Tile> GetBorderTiles()
		{
			var possibleTiles = new List<Tile>();
			foreach (Tile territoryTile in territory)
			{
				var adjacentTiles = territoryTile.GetDirectlyAdjacentTiles();
				foreach (Tile adjacentTile in adjacentTiles)
				{
					if (!possibleTiles.Contains(adjacentTile))
					{
						possibleTiles.Add(adjacentTile);
					}
				}
			}
			return possibleTiles;
		}

		private void CalculateFactionTension()
		{
			foreach(Tile tile in borderTiles)
			{
				var owner = tile.controller;
				if(owner != null && owner != this)
				{
					if (!factionTensions.ContainsKey(owner))
					{
						factionTensions.Add(owner, 0);
					}
					factionTensions[owner] += (owner.territory.Count / 10 * owner.factionPressureModifier.Modified);
				}
			}

			Faction warableFaction = null;
			foreach(var pair in factionTensions)
			{
				if(pair.Value > factionPressureThreshold.Modified)
				{
					//consider war
					warableFaction = pair.Key;
					break;
					//if war probably lower other attentions so we dont get multiple wars from the same factions
				}
			}

			if(warableFaction != null)
			{
				world.AttemptWar(this, warableFaction);
			}
		}

		public void ModifyFactionTension(Faction faction, float amount)
		{
			if(factionTensions.ContainsKey(faction))
			{
				factionTensions[faction] += amount;
			}
		}

		public void ModifyFactionTensionByPercentage(Faction faction, float amount)
		{
			if (factionTensions.ContainsKey(faction))
			{
				factionTensions[faction] *= (amount/100.0f);
			}
		}

		public void SetFactionTension(Faction faction, float amount)
		{
			if (factionTensions.ContainsKey(faction))
			{
				factionTensions[faction] = amount;
			}
		}

		public void AddCity(City city)
		{
			if (!cities.Contains(city))
			{
				cities.Add(city);

				List<Vector2> cityLocations = new List<Vector2>();
				foreach (City c in cities)
				{
					cityLocations.Add(c.tile.GetWorldPosition());
				}

				maxBorderPolygon = SpacialMath.JarvisMarch(cityLocations);
				if (maxBorderPolygon != null)
				{
					nextExpansions.Clear();

					foreach (Tile tile in GetEmptyBorderTiles())
					{
						var tilePos = tile.GetWorldPosition();
						if (maxBorderPolygon.PointInPolygon(tilePos.x, tilePos.y))
						{
							nextExpansions.Add(tile);
						}
					}
				}
			}
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

		public City GetNearestCityToTile(Tile tile)
		{
			if(!territory.Contains(tile))
			{
				return null;
			}

			var closestCity = cities[0];
			var currentDistance = Tile.GetDistanceBetweenTiles(tile, closestCity.tile);
			for(int i = 1; i < cities.Count; i++)
			{
				var checkDistance = Tile.GetDistanceBetweenTiles(tile, cities[i].tile);
				if (checkDistance < currentDistance)
				{
					closestCity = cities[i];
					currentDistance = checkDistance;
				}
			}
			return closestCity;
		}

		public int GetDistanceToNearestCity(Tile tile)
		{
			if (!territory.Contains(tile))
			{
				return int.MaxValue;
			}

			var closestCity = cities[0];
			var currentDistance = Tile.GetDistanceBetweenTiles(tile, closestCity.tile);
			for (int i = 1; i < cities.Count; i++)
			{
				var checkDistance = Tile.GetDistanceBetweenTiles(tile, cities[i].tile);
				if (checkDistance < currentDistance)
				{
					closestCity = cities[i];
					currentDistance = checkDistance;
				}
			}
			return currentDistance;
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
			for(int i = 0; i < cities.Count; i++)
			{
				count += cities[i].population;
			}
			return count;
		}

		private void SetStartingStats()
		{
			actionsPerTurn = new ModifiedInt(1);

			birthRate = new ModifiedFloat(AVERAGE_BIRTH_RATE);
			deathRate = new ModifiedFloat(AVERAGE_DEATH_RATE);
			foodProductionPerWorker = new ModifiedFloat(AVERAGE_FOOD_PRODUCTION);
			spoilageRate = new ModifiedFloat(AVERAGE_SPOILAGE_RATE);
			maxFoodByLand = new ModifiedFloat(MAX_FOOD_BY_LAND);
			maxBurgeoningTension = new ModifiedFloat(MAX_BURGEONING_TENSION);
			rebellionChance = new ModifiedFloat(BASE_REBELLION_CHANCE);
			factionPressureModifier = new ModifiedFloat(1.0f);
			factionPressureThreshold = new ModifiedFloat(BASE_FACTION_PRESSURE_THRESHOLD);

			recruitmentRate = new ModifiedFloat(0);
		}

		private void UpdateStats()
		{
			actionsPerTurn.AdvanceTime();

			birthRate.AdvanceTime();
			deathRate.AdvanceTime();
			foodProductionPerWorker.AdvanceTime();
			spoilageRate.AdvanceTime();
			maxFoodByLand.AdvanceTime();
			maxBurgeoningTension.AdvanceTime();
			rebellionChance.AdvanceTime();
			factionPressureModifier.AdvanceTime();
			factionPressureThreshold.AdvanceTime();

			recruitmentRate.AdvanceTime();

			government.UpdateFactionUsingPassiveTraits(this);
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
			actionsRemaining = actionsPerTurn.Modified;
			foodProducedThisTurn = 0;
			borderTiles = GetBorderTiles();
		}

		public void CheckForDestruction()
		{
			if(cities.Count <= 0)
			{
				FactionGenerator.DestroyFaction(this);
			}
		}

		private void OnRecievedCityDestroyed(LandmarkDestroyedEvent simEvent)
		{
			if (simEvent.landmark is City city)
			{
				if (cities.Contains(city))
				{
					RemoveLandmark(city.tile, city);
				}
			}
		}

		private void SubscribeToEvents()
		{
			EventManager.Instance.AddEventHandler<LandmarkDestroyedEvent>(OnRecievedCityDestroyed);
		}

		public override string ToString()
		{
			return name;
		}
	}
}