using Game.Creatures;
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

		public List<AdventureEncounterObject> evergreenEncounters;
		public List<AdventureEncounterObject> availableEncounters;
		public List<AdventureEncounterObject> usedEncounters;

		public List<MonsterData> monsterData;

		public AdventureService()
		{
			Setup();
		}

		public void AddAvailableEncounter(AdventureEncounterObject encounter)
		{
			availableEncounters.Add(encounter);
		}

		private void Setup()
		{
			evergreenEncounters = new List<AdventureEncounterObject>();
			availableEncounters = new List<AdventureEncounterObject>();
			usedEncounters = new List<AdventureEncounterObject>();
			monsterData = new List<MonsterData>();

			var encountersPath = "ScriptableObjects/Encounters";

			evergreenEncounters.AddRange(Resources.LoadAll(encountersPath, typeof(AdventureEncounterObject)).Cast<AdventureEncounterObject>().ToList());
			OutputLogger.Log(string.Format("{0} encounters loaded.", evergreenEncounters.Count));

			var monstersPath = "ScriptableObjects/Monsters";
			monsterData.AddRange(Resources.LoadAll(monstersPath, typeof(MonsterData)).Cast<MonsterData>().ToList());
			OutputLogger.Log(string.Format("{0} monsters loaded.", monsterData.Count));
		}
	}
}
