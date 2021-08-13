using System.Collections;
using UnityEngine;
using Game.Factions;

namespace Game.Data.EventHandling
{
	public class FactionCreatedEvent : ISimulationEvent
	{
		public FactionSimulator faction;
		public FactionCreatedEvent(FactionSimulator faction)
		{
			this.faction = faction;
		}
	}
}