using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.WorldGeneration;
using Game.Enums;
using Game.Factions;
using Game.Data.EventHandling;

public static class PersonGenerator
{
	public static void RegisterPerson(Person person)
	{
		EventManager.Instance.Dispatch(new CreatureCreatedEvent(person));
	}

	public static void HandleDeath(Person person, string cause)
	{
		var simEvent = new CreatureDiedEvent(person, cause);
		EventManager.Instance.Dispatch(simEvent);
	}
}
