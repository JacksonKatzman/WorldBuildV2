using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public interface SpecialAction
	{
		object Target { get; }

		void Invoke(ISimulationEvent simEvent);
	}
}