using Game.Data.EventHandling.EventRecording;
using Game.Data.EventHandling;
using Game.Factions;
using Game.Generators;
using Game.Generators.Items;
using Game.Landmarks;
using Game.WorldGeneration;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;

namespace Game.Data.EventHandling
{
	public static class WorldEvents
	{ 
		public static void TestWorldEvent_50(World world)
		{
			OutputLogger.LogFormat("Test World Event.", Game.Enums.LogSource.EVENT);

			//Have effect on some aspect of the world

			//Need to what happened, who was involved, outcome, etc to an event to be saved
			//We can then use types or something to generate a cause later ie: Posssible types - divine, mage, etc
		}

		public static void MeteorStrike_01(World world)
		{
			OutputLogger.LogFormat("METEOR EVENT", Game.Enums.LogSource.EVENT);
			var record = new EventRecord();

			//Get strike location
			var chosenTile = world.GetRandomTile();

			//Decide what the meteor is composed of
			var materialUse = SimRandom.RandomFloat01() > 0.5f ? MaterialUse.Construction : MaterialUse.Forging;
			var material = DataManager.Instance.MaterialGenerator.GetRandomMaterialByUse(materialUse, true);

			Relic relic = null;
			var relicChance = SimRandom.RandomRange(0, 10);
			if ((relicChance -= 2) <= 0)
			{
				relic = ItemGenerator.GenerateRelic(new List<MaterialUse> { MaterialUse.Forging }, SimRandom.RandomRange(0,2), SimRandom.RandomRange(0, 2));
			}

			record.AddContext("A meteor is spotted streaking through the nightsky.");

			//Decide whether it strikes or is averted somehow
			var aversionChance = SimRandom.RandomRange(0, 10);
			if((aversionChance -= 2) <= 0)
			{
				//spawn great person who magically prevents meteor and pockets relic if there is one
				var person = new Person(new List<RoleType> { RoleType.MAGIC_USER });
				PersonGenerator.RegisterPerson(person);

				record.AddContext("By mere happenstance, the wizard {0} was near enough to the point of collision to slow the meteor, bringing it harmlessly to rest in a nearby meadow.", person);

				if(relic != null)
				{
					person.inventory.Add(relic);
					record.AddContext("From within the meteor, {0} recovered {1}, a magical artifact of unknown origin.", person, relic);
				}
			}
			else
			{
				//Spawn landmark crater
				MeteorCrater crater;
				if(relic != null)
				{
					crater = new MeteorCrater(material, relic);
					relic.Activate();

					record.AddContext("The meteor, made of solid {0}, crashes into the earth at {1}, creating the crater: {2}. Buried deep within the crater, {3} lies unclaimed.", material, chosenTile, crater, relic);
				}
				else
				{
					crater = new MeteorCrater(material);
					record.AddContext("The meteor, made of solid {0}, crashes into the earth at {1}, creating the crater: {2}.", material, chosenTile, crater);
				}
				//Decide what faction if any claims it or does anything with it
				crater.faction = chosenTile.controller;

				//handle destroying other landmarks if they existed
				for (int i = 0; i < chosenTile.landmarks.Count; i++)
				{
					LandmarkGenerator.DestroyLandmark(chosenTile.landmarks[i]);
					record.AddContext("The meteor's collision destroyed {0}.", chosenTile.landmarks[i]);
				}

				chosenTile.landmarks.Clear();

				LandmarkGenerator.RegisterLandmark(chosenTile, crater);
				SimulationManager.Instance.eventRecorder.eventRecords.Add(record);
			}
		}

		public static void Plague_02(World world)
		{
			if (world.factions.Count > 3)
			{
				OutputLogger.LogFormat("Plague Event", Game.Enums.LogSource.EVENT);

				var faction = SimRandom.RandomEntryFromList(world.factions);
				var city = SimRandom.RandomEntryFromList(faction.cities);
				var survivalRate = SimRandom.RandomRange(20, 80) / 100.0f;

				WorldEventHelpers.Plague_Helper(world, city, survivalRate, new List<City>());
			}
		}
	}

	public static class WorldEventHelpers
	{
		public static void Plague_Helper(World world, City city, float survivalRate, List<City> visitedCities)
		{
			city.population = (int)(survivalRate * city.population);
			visitedCities.Add(city);

			if(survivalRate < 1.0f && survivalRate > 0.0f)
			{
				var surroundingTiles = city.tile.GetAllTilesInRadius(5);
				var surroundingCities = new List<City>();
				foreach(var tile in surroundingTiles)
				{
					surroundingCities.AddRange(tile.GetCities());
				}

				surroundingCities.RemoveAll(c => visitedCities.Contains(c));
				if(surroundingCities.Count > 0)
				{
					var nextCity = SimRandom.RandomEntryFromList(surroundingCities);

					survivalRate += SimRandom.RandomRange(-10, 20) / 100.0f;
					world.ongoingEvents.Add(new OngoingEvent(1, () => { Plague_Helper(world, nextCity, survivalRate, visitedCities); }));
				}
			}
		}
	}
}