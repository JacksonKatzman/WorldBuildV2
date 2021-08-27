using Game.Data.EventHandling.EventRecording;
using System.Collections;
using UnityEngine;

namespace Game.Holiday
{
	public class Holiday : IRecordable
	{
		private string name;

		public string Name => name;

		public Holiday()
		{
			name = NameGenerator.GenerateHolidayName();
		}
	}
}