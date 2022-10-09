using System.Collections;
using UnityEngine;
using Game.Factions;

namespace Game.Data.EventHandling
{
	public class FactionCreatedEvent : ISimulationEvent
	{
		public OldFaction faction;
		public FactionCreatedEvent(OldFaction faction)
		{
			this.faction = faction;
		}
	}
}