using Game.Data.EventHandling.EventRecording;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ModularEvents
{
	public class AddRecordContextAction : IEventActionContext
	{
		[SerializeReference]
		IEventContext contextType;

		public string recordContext;
		public void Activate(Person person, EventRecord record)
		{
			record.AddContext(recordContext, person);
		}

		//public static List<>
	}
}