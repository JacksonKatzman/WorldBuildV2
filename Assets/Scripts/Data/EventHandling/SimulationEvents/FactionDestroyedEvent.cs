using Game.Factions;
using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public class FactionDestroyedEvent : ISimulationEvent
	{
		public Faction faction;

		public FactionDestroyedEvent(Faction faction)
		{
			this.faction = faction;
		}
	}
}