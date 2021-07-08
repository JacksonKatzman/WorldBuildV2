using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public class SpecificSpecialAction<T> : SpecialAction where T : ISimulationEvent
	{
		public object Target { get { return Handler; } }
		public System.Action<T> Handler { get; set; }

		public void Invoke(ISimulationEvent simEvent)
		{
			Handler.Invoke((T)simEvent);
		}
	}
}