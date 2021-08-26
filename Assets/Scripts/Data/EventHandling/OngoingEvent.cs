using System;
using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public class OngoingEvent
	{
		public int duration;
		public Action eventAction;

		public OngoingEvent(int duration, Action eventAction)
		{
			this.duration = duration;
			this.eventAction = eventAction;
		}
	}
}