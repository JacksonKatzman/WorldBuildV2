using Game.Creatures;
using Game.Data.EventHandling.EventRecording;
using Game.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ModularEvents
{
	//[System.Serializable]
	public class HostileCreatureContext : BaseEventContext, IEventContext
	{
		public int minPoints;
		public int maxPoints;

		public bool legendary;
		public bool landDwelling;
		public List<CreatureType> allowedTypes;

		private MonsterStats lastMonster;

		public void RunEvent(Person person, EventRecord record)
		{
			var reasonStrings = new List<string>
			{
				"Due to the past several years of heavy rainfall and comfortable conditions there has been a boom in the local monster populations. Small packs are being displaced from their traditional homes and hunting grounds forcing them into populated and high traffic areas.",
				"Due to the past several years of famine and difficult conditions monsters are roaming further and further from their traditional hunting grounds seeking resources, forcing them into populated and high traffic areas.",
				"Due to the worsening state of the war, there aren't enough soliders and guards to secure the roads and outlying towns, so clever monsters have begun to take advantage of the lax security to prey on the citizenry."
			};

			record.AddContext(SimRandom.RandomEntryFromList(reasonStrings));

			var creature = DataManager.Instance.GetRandomCreature(legendary, landDwelling, allowedTypes.ToArray());
			var numPoints = SimRandom.RandomRange(minPoints, maxPoints);
			var pointValue = (int)creature.type + 1;
			var numCreatures = numPoints / pointValue;
			var city = SimRandom.RandomEntryFromList(person.faction.cities);
			lastMonster = creature;

			record.AddContext("As a result, a horde of " + numCreatures + " {0}'s has been raiding the villages surrounding {1}.", creature, city);
		}
		public int GetRollMargin(Person person, StatType statType)
		{
			var personRoll = SimRandom.RollXDY(1, 20) + person.stats.GetModValue(statType);
			var creatureRoll = SimRandom.RollXDY(1, 20) + new CreatureStats(lastMonster.stats.BuildDictionary()).GetModValue(statType);
			return personRoll - creatureRoll;
		}
	}
}