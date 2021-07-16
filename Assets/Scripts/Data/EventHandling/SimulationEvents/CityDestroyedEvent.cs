using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public class CityDestroyedEvent : ISimulationEvent
	{
		public City city;

		public CityDestroyedEvent(City city)
		{
			this.city = city;
		}
	}
}