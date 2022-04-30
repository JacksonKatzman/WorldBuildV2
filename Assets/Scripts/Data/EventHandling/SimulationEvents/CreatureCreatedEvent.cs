using Game.Creatures;
using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public class CreatureCreatedEvent : ISimulationEvent
	{
		public ICreature creature;

		public CreatureCreatedEvent(ICreature creature)
		{
			this.creature = creature;
		}
	}
}