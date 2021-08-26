using System.Collections;
using UnityEngine;
using Game.Factions;

namespace Game.Data.EventHandling
{
	public class FactionCreatedEvent : ISimulationEvent
	{
		public Faction faction;
		public FactionCreatedEvent(Faction faction)
		{
			this.faction = faction;
		}
	}
}