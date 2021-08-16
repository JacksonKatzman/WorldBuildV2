using Game.Data.EventHandling.EventRecording;
using Game.Factions;
using Game.Generators;
using Game.Generators.Items;
using Game.Landmarks;
using Game.WorldGeneration;
using System.Collections.Generic;
using UnityEngine;

namespace Data.EventHandling
{
	public abstract class WorldEvents
	{ 
		public static void TestWorldEvent(World world)
		{
			OutputLogger.LogFormat("Test World Event.", Game.Enums.LogSource.EVENT);

			//Have effect on some aspect of the world

			//Need to what happened, who was involved, outcome, etc to an event to be saved
			//We can then use types or something to generate a cause later ie: Posssible types - divine, mage, etc
		}

		public static void MeteorStrike(World world)
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

			//Decide whether it strikes or is averted somehow
			var aversionChance = SimRandom.RandomRange(0, 10);
			if((aversionChance -= 2) <= 0)
			{
				//spawn great person who magically prevents meteor and pockets relic if there is one
				var person = PersonGenerator.GeneratePerson();
				if(relic != null)
				{
					person.inventory.Add(relic);
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
				}
				else
				{
					crater = new MeteorCrater(material);
				}
				//Decide what faction if any claims it or does anything with it
				crater.faction = chosenTile.controller;

				//handle destroying other landmarks if they existed
				for (int i = 0; i < chosenTile.landmarks.Count; i++)
				{
					LandmarkGenerator.DestroyLandmark(chosenTile.landmarks[i]);
				}

				chosenTile.landmarks.Clear();

				LandmarkGenerator.RegisterLandmark(chosenTile, crater);
			}
		}
	}
}