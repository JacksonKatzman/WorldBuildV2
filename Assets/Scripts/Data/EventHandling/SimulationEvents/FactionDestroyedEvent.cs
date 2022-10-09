using Game.Factions;
using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public class FactionDestroyedEvent : ISimulationEvent
	{
		public OldFaction faction;

		public FactionDestroyedEvent(OldFaction faction)
		{
			this.faction = faction;
		}
	}
}