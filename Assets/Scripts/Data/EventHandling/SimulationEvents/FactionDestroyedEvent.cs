using Game.Factions;
using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public class FactionDestroyedEvent : ISimulationEvent
	{
		public FactionSimulator faction;

		public FactionDestroyedEvent(FactionSimulator faction)
		{
			this.faction = faction;
		}
	}
}