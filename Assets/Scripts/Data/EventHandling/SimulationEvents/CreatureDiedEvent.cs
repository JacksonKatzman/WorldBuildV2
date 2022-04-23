using Game.Creatures;
using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public class CreatureDiedEvent : ISimulationEvent
	{
		public ICreature creature;
		public string cause;

		public CreatureDiedEvent(ICreature creature, string cause)
		{
			this.creature = creature;
			this.cause = cause;
		}
	}
}