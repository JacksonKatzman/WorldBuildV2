using Game.Data.EventHandling.EventRecording;
using System.Collections;
using UnityEngine;

namespace Game.Incidents
{
	public interface ITakeableAction
	{
		public IRecordable ActionTaker
		{
			set;
		}
	}
}