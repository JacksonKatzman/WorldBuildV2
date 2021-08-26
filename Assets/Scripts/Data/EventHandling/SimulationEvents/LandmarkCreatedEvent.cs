using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public class LandmarkCreatedEvent : ISimulationEvent
	{
		public Landmark landmark;

		public LandmarkCreatedEvent(Landmark landmark)
		{
			this.landmark = landmark;
		}
	}
}