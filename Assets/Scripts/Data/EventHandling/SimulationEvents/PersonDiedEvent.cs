using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public class PersonDiedEvent : ISimulationEvent
	{
		public Person person;
		public string cause;

		public PersonDiedEvent(Person person, string cause)
		{
			this.person = person;
			this.cause = cause;
		}
	}
}