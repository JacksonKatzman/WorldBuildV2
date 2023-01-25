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

		public List<AdventureEncounter> encounters;
		public List<AdventureEncounter> usedEncounters;

		public AdventureService()
		{
			Setup();
		}

		private void Setup()
		{
			encounters = new List<AdventureEncounter>();
			usedEncounters = new List<AdventureEncounter>();

			var path = "ScriptableObjects/Encounters";

			encounters.AddRange(Resources.LoadAll(path, typeof(AdventureEncounter)).Cast<AdventureEncounter>().ToList());
			OutputLogger.Log(string.Format("{0} encounters loaded.", encounters.Count));
		}
	}
}
