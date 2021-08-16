using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public class LandmarkDestroyedEvent : ISimulationEvent
	{
		public Landmark landmark;

		public LandmarkDestroyedEvent(Landmark landmark)
		{
			this.landmark = landmark;
		}
	}
}