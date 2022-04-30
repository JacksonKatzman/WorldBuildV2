using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

namespace Game.Data.EventHandling.EventRecording
{
	public class EventRecord
	{
		public List<IRecordable> Records => records;
		public string WrittenRecord => writtenRecord;

		private List<IRecordable> records;
		private string writtenRecord;

		public EventRecord()
		{
			records = new List<IRecordable>();
			writtenRecord = string.Empty;
		}

		public void AddContext(string context, params IRecordable[] recordables)
		{
			for(int i = 0; i < recordables.Length; i++)
			{
				var recordable = recordables[i];

				if(!records.Contains(recordable))
				{
					records.Add(recordable);
				}

				var toReplace = "{" + i + "}";
				var replaceWith = "{" + records.IndexOf(recordable) + "}";
				context.Replace(toReplace, replaceWith);
			}

			writtenRecord += (context + " ");
		}

		public string GetContext()
		{
			return string.Format(writtenRecord, records.Select(r => r.Name).ToArray());
		}
	}
}