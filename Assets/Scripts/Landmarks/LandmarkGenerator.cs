using Game.Data.EventHandling;
using Game.Enums;
using Game.Factions;
using Game.WorldGeneration;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Generators
{
	public static class LandmarkGenerator
	{
		private static float STARTING_FOOD = 100.0f;
		private static int STARTING_POPULATION = 100;

		public static City SpawnCity(Tile tile, Faction faction, float foodAmount, int population)
		{
			var city = new City(tile, faction, foodAmount, population);
			tile.landmarks.Add(city);

			EventManager.Instance.Dispatch(new LandmarkCreatedEvent(city));

			return city;
		}

		public static City SpawnCity(Tile tile, Faction faction)
		{
			return SpawnCity(tile, faction, STARTING_FOOD, STARTING_POPULATION);
		}

		public static void RegisterLandmark(Tile tile, Landmark landmark)
		{
			tile.landmarks.Add(landmark);

			EventManager.Instance.Dispatch(new LandmarkCreatedEvent(landmark));
		}

		public static void DestroyLandmark(Landmark landmark)
		{
			EventManager.Instance.Dispatch(new LandmarkDestroyedEvent(landmark));
		}

		public static void DestroyCity(City city)
		{
			EventManager.Instance.Dispatch(new LandmarkDestroyedEvent(city));
		}

		public static Tile FindSuitableCityLocation(Tile tile, int innerRadius, int outerRadius, float targetFertility, float targetLandAvailability, int minCityDistance, Faction faction = null)
		{
			var possibleTiles = tile.GetAllTilesInRing(outerRadius, innerRadius);
			var acceptedTiles = new List<Tile>();

			var worldCities = tile.world.Cities;

			foreach(var possibleTile in possibleTiles)
			{
				var closestCity = Tile.GetClosestCityToTile(worldCities, possibleTile);
				if(closestCity != null && Tile.GetDistanceBetweenTiles(closestCity.tile, possibleTile) < minCityDistance)
				{
					continue;
				}

				var tileController = tile.controller;
				var uncontrolled = (tileController == faction || tileController == null);

				if(tile.baseFertility >= targetFertility && possibleTile.GetCities().Count == 0 && uncontrolled && tile.biome.availableLand >= targetLandAvailability)
				{
					acceptedTiles.Add(possibleTile);
				}
			}

			return acceptedTiles.Count > 0 ? SimRandom.RandomEntryFromList(acceptedTiles) : null;
		}
	}
}