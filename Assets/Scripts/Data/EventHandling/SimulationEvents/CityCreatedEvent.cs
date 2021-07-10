using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public class CityCreatedEvent : ISimulationEvent
	{
		public City city;

		public CityCreatedEvent(City city)
		{
			this.city = city;
		}
	}
}