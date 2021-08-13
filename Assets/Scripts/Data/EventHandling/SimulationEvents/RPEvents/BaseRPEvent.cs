using System;
using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public class BaseRPEvent : ISimulationEvent
	{
		public Person person;
		public string eventDetails;
		public Action effect;

		public BaseRPEvent(Person person, string eventDetails, Action effect = null)
		{
			this.person = person;
			this.eventDetails = eventDetails;
			this.effect = effect;
		}
	}
}