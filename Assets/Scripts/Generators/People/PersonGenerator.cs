using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.WorldGeneration;
using Game.Enums;
using Game.Factions;
using Game.Data.EventHandling;

public static class PersonGenerator
{
	public static Person GeneratePerson(FactionSimulator faction, Vector2Int ageRange, Gender gender, int startingInfluence, LeadershipStructureNode office = null)
	{
		var person = new Person(SimRandom.RandomRange(ageRange.x, ageRange.y), gender, startingInfluence, office);
		person.faction = faction;

		EventManager.Instance.Dispatch(new PersonCreatedEvent(person));

		return person;
	}

	public static void HandleDeath(Person person, string cause)
	{
		var simEvent = new PersonDiedEvent(person, cause);
		EventManager.Instance.Dispatch(simEvent);
	}
}
