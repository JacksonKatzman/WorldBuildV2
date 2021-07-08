using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.WorldGeneration;
using Game.Enums;
using Game.Factions;

public static class PersonGenerator
{
    private static World world => WorldHandler.Instance.World;

    public static Person GeneratePerson(Faction faction, Vector2Int ageRange, Gender gender)
	{
		var person = new Person(SimRandom.RandomRange(ageRange.x, ageRange.y), gender);
		person.faction = faction;
		world.AddPerson(person);

		return person;
	}

	public static void HandleDeath(Person person)
	{
		//person.faction?.
		world.RemovePerson(person);
	}
}
