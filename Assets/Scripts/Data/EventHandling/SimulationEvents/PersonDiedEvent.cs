using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public class PersonDiedEvent : ISimulationEvent
	{
		public Person person;

		public PersonDiedEvent(Person person)
		{
			this.person = person;
		}
	}
}