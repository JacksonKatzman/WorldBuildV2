using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public class PersonCreatedEvent : ISimulationEvent
	{
		public Person person;

		public PersonCreatedEvent(Person person)
		{
			this.person = person;
		}
	}
}