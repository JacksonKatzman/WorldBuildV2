using Game.Data.EventHandling.EventRecording;
using Game.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public static class EventCatalyst
	{
		public static EventRecord MonsterGathering(City location, params CreatureType[] typesArray)
		{
			var reasonStrings = new List<string>
			{
				"Due to the past several years of heavy rainfall and comfortable conditions there has been a boom in the local monster populations. Small packs are being displaced from their traditional homes and hunting grounds forcing them into populated and high traffic areas.",
				"Due to the past several years of famine and difficult conditions monsters are roaming further and further from their traditional hunting grounds seeking resources, forcing them into populated and high traffic areas.",
				"Due to the worsening state of the war, there aren't enough soliders and guards to secure the roads and outlying towns, so clever monsters have begun to take advantage of the lax security to prey on the citizenry." 
			};

			var record = new EventRecord();
			record.AddContext(SimRandom.RandomEntryFromList(reasonStrings));

			var creature = DataManager.Instance.GetRandomCreature(false, true, typesArray);
			var numPoints = SimRandom.RandomRange(40, 200);
			var pointValue = (int)creature.type + 1;
			var numCreatures = numPoints / pointValue;

			record.AddContext("As a result, a horde of " + numCreatures + " {0}'s has been raiding the villages surrounding {1}.");
			SimulationManager.Instance.eventRecorder.eventRecords.Add(record);

			return record;
		}
	}
}