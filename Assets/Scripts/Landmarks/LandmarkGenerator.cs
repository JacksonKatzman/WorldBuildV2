using Game.Data.EventHandling;
using Game.Enums;
using Game.Factions;
using Game.WorldGeneration;
using System.Collections;
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

			EventManager.Instance.Dispatch(new CityCreatedEvent(city));

			return city;
		}

		public static City SpawnCity(Tile tile, Faction faction)
		{
			return SpawnCity(tile, faction, STARTING_FOOD, STARTING_POPULATION);
		}

		public static void DestroyCity(City city)
		{
			EventManager.Instance.Dispatch(new CityDestroyedEvent(city));
		}

		public static bool IsSuitableCityLocation(Tile tile, float targetFertility, float targetLandAvailability, Faction faction = null)
		{
			var tileController = tile.world.GetFactionThatControlsTile(tile);
			var uncontrolled = (tileController == faction || tileController == null);

			var farAwayEnough = true;
			if (faction != null)
			{
				foreach (City city in faction.cities)
				{
					if(Tile.GetDistanceBetweenTiles(tile, city.tile) < 5)
					{
						farAwayEnough = false;
						break;
					}
				}
			}

			return (tile.baseFertility >= targetFertility && tile.GetNumberOfCities() == 0 && uncontrolled && tile.biome.availableLand >= targetLandAvailability && farAwayEnough);
		}
	}
}