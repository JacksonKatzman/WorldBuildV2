using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using Game.WorldGeneration;
using System;

namespace Game.Data.EventHandling.EventRecording
{
	public class EventRecorder
	{
		private static World world => SimulationManager.Instance.World;
		private EventManager events => EventManager.Instance;
		private Dictionary<Person, List<EventLog>> personLogs;

		public EventRecorder()
		{
			personLogs = new Dictionary<Person, List<EventLog>>();

			Setup();

			SubscribeToEvents();
		}

		private void RecordPerson(Person person)
		{
			var localDir = Application.persistentDataPath;
			var factionName = person.faction == null ? "Unaffiliated" : person.faction.Name;
			var cleanedFactionName = factionName.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
			var targetDir = localDir + @"\Person Logs\" + cleanedFactionName;

			if(!Directory.Exists(targetDir))
			{
				Directory.CreateDirectory(targetDir);
			}

			var cleanedPersonName = person.Name.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
			targetDir += @"\" + cleanedPersonName + ".txt";
			var log = personLogs[person];
			var contents = new string[log.Count];

			for(int i = 0; i < log.Count; i++)
			{
				contents[i] = string.Format("{0}: - {1}", log[i].year, log[i].simEvent.eventDetails);
			}

			WriteToFile(targetDir, contents);
		}

		private void WriteToFile(string path, string[] contents)
		{
			if (!File.Exists(path))
			{
				File.WriteAllLines(path, contents, Encoding.UTF8);
			}
		}

		private void SubscribeToEvents()
		{
			events.AddEventHandler<PersonCreatedEvent>(OnPersonCreated);
			events.AddEventHandler<PersonDiedEvent>(OnPersonDied);
			events.AddEventHandler<BaseRPEvent>(OnRPEvent);
		}

		private void OnPersonCreated(PersonCreatedEvent simEvent)
		{
			personLogs.Add(simEvent.person, new List<EventLog>());
			personLogs[simEvent.person].Add(new EventLog(world.yearsPassed, new BaseRPEvent(simEvent.person, string.Format("{0} rose to prominence.", simEvent.person.Name.Replace("\r\n", "").Replace("\r", "").Replace("\n", "")))));
		}

		private void OnPersonDied(PersonDiedEvent simEvent)
		{
			personLogs[simEvent.person].Add(new EventLog(world.yearsPassed, new BaseRPEvent(simEvent.person, string.Format("{0} died of: {1}.", simEvent.person.Name.Replace("\r\n", "").Replace("\r", "").Replace("\n", ""), simEvent.cause))));
			RecordPerson(simEvent.person);
		}

		private void OnRPEvent(BaseRPEvent simEvent)
		{
			personLogs[simEvent.person].Add(new EventLog(world.yearsPassed, simEvent));
		}

		private void Setup()
		{
			var localDir = Application.persistentDataPath;
			var targetDir = localDir + @"\Person Logs";
			if (Directory.Exists(targetDir))
			{
				DirectoryInfo directory = new DirectoryInfo(targetDir);

				foreach (FileInfo file in directory.GetFiles())
				{
					file.Delete();
				}

				foreach (DirectoryInfo dir in directory.GetDirectories())
				{
					dir.Delete(true);
				}
			}
			else
			{
				Directory.CreateDirectory(targetDir);
			}
			OutputLogger.LogFormat(targetDir, Enums.LogSource.PROFILE);
		}
	}

	public class EventLog
	{
		public int year;
		public BaseRPEvent simEvent;

		public EventLog(int year, BaseRPEvent simEvent)
		{
			this.year = year;
			this.simEvent = simEvent;
		}
	}
}