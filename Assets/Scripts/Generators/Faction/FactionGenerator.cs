using Game.Data.EventHandling;
using Game.Enums;
using Game.Factions;
using Game.Races;
using Game.WorldGeneration;
using System.Collections;
using UnityEngine;

namespace Game.Generators
{
	public static class FactionGenerator
	{
		private static float STARTING_FOOD = 100.0f;
		private static int STARTING_POPULATION = 100;
		private static int MAX_SPAWN_ATTEMPTS = 100;
		public static void SpawnFaction(OldWorld world)
		{
			bool spawned = false;

			int attempts = 0;
			while (!spawned && attempts < MAX_SPAWN_ATTEMPTS)
			{
				var randomTile = world.GetRandomTile();
				var chosenTile = LandmarkGenerator.FindSuitableCityLocation(randomTile, 1, 10, 0.5f, 0.2f, 7);

				if (chosenTile != null)
				{
					SpawnFaction(chosenTile, STARTING_FOOD, STARTING_POPULATION, DataManager.Instance.GetRandomWeightedRace());
					spawned = true;
				}
				attempts++;
			}

			if (attempts >= MAX_SPAWN_ATTEMPTS)
			{
				//Debug.LogFormat("Failed to spawn city in chunk ({0},{1})", coords.x, coords.y);
			}
		}

		public static void SpawnFaction(Tile tile, float foodAmount, int population, Race race)
		{
			var faction = new OldFaction(tile, foodAmount, population, race);
			EventManager.Instance.Dispatch(new FactionCreatedEvent(faction));

			OutputLogger.LogFormat("Spawned faction in chunk ({0},{1}) in tile ({2},{3})).",
						LogSource.WORLDGEN, tile.chunk.coords.x, tile.chunk.coords.y, tile.coords.x, tile.coords.y);
		}

		public static void DestroyFaction(OldFaction faction)
		{
			for(int i = 0; i < faction.territory.Count; i++)
			{
				faction.territory[i].controller = null;
			}
			faction.territory.Clear();
			EventManager.Instance.Dispatch(new FactionDestroyedEvent(faction));
		}
	}
}