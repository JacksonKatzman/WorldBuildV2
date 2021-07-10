using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.WorldGeneration;
using Game.Enums;
using Game.Factions;
using Game.Data.EventHandling;

public static class PersonGenerator
{
    public static Person GeneratePerson(Faction faction, Vector2Int ageRange, Gender gender)
	{
		var person = new Person(SimRandom.RandomRange(ageRange.x, ageRange.y), gender);
		person.faction = faction;

		EventManager.Instance.Dispatch(new PersonCreatedEvent(person));

		return person;
	}

	public static void HandleDeath(Person person)
	{
		var simEvent = new PersonDiedEvent(person);
		EventManager.Instance.Dispatch(simEvent);
	}
}
