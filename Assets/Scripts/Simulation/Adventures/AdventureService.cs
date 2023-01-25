using Game.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Simulation
{
	public class AdventureService
	{
		private static AdventureService instance;
		public static AdventureService Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new AdventureService();
				}
				return instance;
			}
		}

		public List<AdventureEncounter> evergreenEncounters;
		public List<AdventureEncounter> availableEncounters;
		public List<AdventureEncounter> usedEncounters;

		public AdventureService()
		{
			Setup();
		}

		public void AddAvailableEncounter(AdventureEncounter encounter)
		{
			availableEncounters.Add(encounter);
		}

		private void Setup()
		{
			evergreenEncounters = new List<AdventureEncounter>();
			availableEncounters = new List<AdventureEncounter>();
			usedEncounters = new List<AdventureEncounter>();

			var path = "ScriptableObjects/Encounters";

			evergreenEncounters.AddRange(Resources.LoadAll(path, typeof(AdventureEncounter)).Cast<AdventureEncounter>().ToList());
			OutputLogger.Log(string.Format("{0} encounters loaded.", evergreenEncounters.Count));
		}
	}
}
