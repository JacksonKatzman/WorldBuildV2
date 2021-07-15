using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public class BaseRPEvent : ISimulationEvent
	{
		public Person person;
		public string eventDetails;

		public BaseRPEvent(Person person, string eventDetails)
		{
			this.person = person;
			this.eventDetails = eventDetails;
		}
	}
}