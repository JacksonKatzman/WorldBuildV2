using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public class ParameterlessSpecialAction : SpecialAction
	{
		public object Target { get { return Handler; } }
		public System.Action Handler { get; set; }

		public void Invoke(ISimulationEvent simEvent)
		{
			Handler.Invoke();
		}
	}
}