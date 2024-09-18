using Game.Enums;
using Game.Incidents;
using Game.Terrain;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.Simulation
{
	/*
 * Types of Encounters:
 * - Combat
 * - Puzzle
 * - RP
 * - Curiosity?
 * - ??
 */

	//Account for multiple possible solutions

	/*
	 * Encounters contains info about whos involved, (could be people or monsters),
	 * what those things are doing, where they are doing it, and why.
	 * If an encounter is random and not the main part of a hook, these things
	 * are chosen and described when they are found. But I need a way of parsing some
	 * of that information to create a description and objectives for adventure hooks,
	 * in the case that this encounter is the climax. 
	 */

	//Encounter Location Type [Dungeon, Overworld]
	//Encounter Biome Type [Desert vs Forest vs ANY]
	//Encounter Type [Puzzle vs Combat etc]
	//Encounter approx challenge level

	//Encounter description for when encountered
	//Encounter overview for when chosen as hooks main encounter
	//Any fun extras to find while doing encounter (like treasure or historical notes etc)

	//Unrelated but need to set up a system similar to IncidentCriteria for finding threats
	//Includes both monsters and non monster threats

	[CreateAssetMenu(fileName = nameof(AdventureEncounterObject), menuName = "ScriptableObjects/Adventures/" + nameof(AdventureEncounterObject), order = 1)]
	public class AdventureEncounterObject : SerializedScriptableObject, ILocationAffiliated
	{
		[PropertyOrder(-11)]
		public string encounterTitle;
		[PropertyOrder(-10)]
		public int encounterDifficulty;
		[PropertyOrder(-9)]
		public bool majorEncounter;
		[PropertyOrder(-9)]
		public bool skippable;
		[PropertyOrder(-8)]
		public EncounterLocationType encounterLocationType;

		public Location CurrentLocation { get; set; }

		[ValueDropdown("GetEncounterTypes", IsUniqueList = true, DropdownTitle = "Encounter Types"), PropertyOrder(-8)]
		public List<EncounterType> encounterTypes;

		[ValueDropdown("GetBiomeTerrainTypes", IsUniqueList = true, DropdownTitle = "Allowed Biomes"), PropertyOrder(-7)]
		public List<BiomeTerrainType> allowedBiomes;

		//[ListDrawerSettings(HideAddButton = true), PropertyOrder(0)]
		[PropertyOrder(0)]
		public ObservableCollection<IAdventureContextRetriever> contextCriterium = new ObservableCollection<IAdventureContextRetriever>();

		[TextArea(2, 4), PropertyOrder(0)]
		public string encounterBlurb;

		[TextArea(10, 15), PropertyOrder(0)]
		public string encounterSummary;

		//[ListDrawerSettings(HideRemoveButton = true, HideAddButton = true)]
		public ObservableCollection<IAdventureComponent> components = new ObservableCollection<IAdventureComponent>();

		public AdventureEncounterObject()
		{
			encounterTypes = new List<EncounterType>();
			allowedBiomes = new List<BiomeTerrainType>();
		}

        public void OnValidate()
        {
			Debug.OutputLogger.Log("VALIDATING");
			/*
			for(int i = 0; i < contextCriterium.Count; i++)
            {
				contextCriterium[i].RetrieverID = i;
            }
			*/
			//maybe here we can hook up the observable list listener if there isnt one already hooked up
			if (contextCriterium != null)
			{
				contextCriterium.CollectionChanged -= OnCollectionChanged;
				contextCriterium.CollectionChanged += OnCollectionChanged;
			}
        }

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
			var thing = new Dictionary<IAdventureContextRetriever, Tuple<int, int>>();
			if (e.OldItems != null)
			{
				foreach (var item in e.OldItems)
				{
					var removedItem = (IAdventureContextRetriever)item;
					thing.Add(removedItem, new Tuple<int, int>(removedItem.RetrieverID, -1));
				}
			}

			foreach (var c in contextCriterium)
            {
				thing.Add(c, new Tuple<int, int>(c.RetrieverID, c.RetrieverID));
            }

			for(int i = 0; i < contextCriterium.Count; i++)
            {
				var comp = contextCriterium[i];
				thing[comp] = new Tuple<int, int>(thing[comp].Item1, i);
				comp.RetrieverID = i;
            }

			if (e.NewItems != null && e.NewItems.Count > 0)
			{
				return;
			}

			foreach (var pair in thing)
            {
				//if the values of x and y differ, replace all instances of :x} with :y}
				if (pair.Value.Item1 != pair.Value.Item2)
                {
					foreach(var component in components)
                    {
						component.UpdateStuff(pair.Value.Item1, pair.Value.Item2);
                    }
                }
            }
		}

		public bool TryGetContext(int id, out IIncidentContext result)
		{
			result = GetContexts().Find(x => x.ID == id);
			return result != null;
			//return !result.Equals(default(IIncidentContext));
		}

		public bool TryGetContextCriteria(int id, out IAdventureContextRetriever result)
		{
			result = contextCriterium.ToList().Find(x => x.Context.ID == id);
			return result != null;
		}

		private List<IIncidentContext> GetContexts()
        {
			return contextCriterium.Select(x => x.Context).ToList();
        }

		private IEnumerable<EncounterType> GetEncounterTypes()
		{
			return Enum.GetValues(typeof(EncounterType)).Cast<EncounterType>();
		}

		private IEnumerable<BiomeTerrainType> GetBiomeTerrainTypes()
		{
			return Enum.GetValues(typeof(BiomeTerrainType)).Cast<BiomeTerrainType>();
		}
	}
}
