using Game.Enums;
using Game.Incidents;
using Game.Terrain;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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
	public class AdventureEncounterObject : SerializedScriptableObject
	{
		[PropertyOrder(-10)]
		public string encounterTitle;
		[PropertyOrder(-9)]
		public EncounterLocationType encounterLocationType;

		public Location Location { get; set; }

		[ValueDropdown("GetEncounterTypes", IsUniqueList = true, DropdownTitle = "Encounter Types"), PropertyOrder(-8)]
		public List<EncounterType> encounterTypes;

		[ValueDropdown("GetBiomeTerrainTypes", IsUniqueList = true, DropdownTitle = "Allowed Biomes"), PropertyOrder(-7)]
		public List<BiomeTerrainType> allowedBiomes;

		[ListDrawerSettings(HideAddButton = true), PropertyOrder(0)]
		public List<IAdventureContextCriteria> contextCriterium;

		[TextArea(2, 4), PropertyOrder(0)]
		public string encounterBlurb;

		[TextArea(10, 15), PropertyOrder(0)]
		public string encounterSummary;

		[TextArea(15, 20), PropertyOrder(0)]
		public string encounterApproach;

		[PropertyOrder(0), ListDrawerSettings(CustomAddFunction = "AddAct")]
		public List<AdventureEncounterAct> encounterActs;

		[ListDrawerSettings(HideRemoveButton = true, HideAddButton = true)]
		public List<IAdventureComponent> components;

		public AdventureEncounterObject()
		{
			encounterTypes = new List<EncounterType>();
			allowedBiomes = new List<BiomeTerrainType>();
			encounterActs = new List<AdventureEncounterAct>();
			contextCriterium = new List<IAdventureContextCriteria>();
		}

		private void UpdateContainerIDs()
		{
			for (int i = 0; i < contextCriterium.Count; i++)
			{
				contextCriterium[i].ContextID = "{" + i + "}";
			}
		}

		private IEnumerable<EncounterType> GetEncounterTypes()
		{
			return Enum.GetValues(typeof(EncounterType)).Cast<EncounterType>();
		}

		private IEnumerable<BiomeTerrainType> GetBiomeTerrainTypes()
		{
			return Enum.GetValues(typeof(BiomeTerrainType)).Cast<BiomeTerrainType>();
		}

		private void AddAct()
		{
			encounterActs.Add(new AdventureEncounterAct());
		}

		private void RemoveCriteria(int index)
		{
			contextCriterium.RemoveAt(index);
			UpdateContainerIDs();
		}

		[ButtonGroup("1"), PropertyOrder(-1)]
		private void AddMonster()
		{
			contextCriterium.Add(new MonsterCriteria());
			UpdateContainerIDs();
		}

		[ButtonGroup("1"), PropertyOrder(-1)]
		private void AddPerson()
		{
			contextCriterium.Add(new AdventureContextCriteria<Person>());
			UpdateContainerIDs();
		}
	}

	public interface IAdventureComponent
	{
		public bool Completed { get; set; }
		public int ComponentID { get; set; }
		public void UpdateComponentID(ref int nextID, List<int> removedIds = null);
		public List<int> GetRemovedIds();
	}

	[HideReferenceObjectPicker]
	public abstract class AdventureComponent : IAdventureComponent
	{
		virtual public bool Completed { get; set; }
		[SerializeField, ReadOnly]
		public int ComponentID { get; set; }
		virtual public void UpdateComponentID(ref int nextID, List<int> removedIds = null)
		{
			ComponentID = nextID;
			nextID++;
		}

		virtual public List<int> GetRemovedIds()
		{
			return new List<int>() { ComponentID };
		}

		protected void UpdateLinkID(ref int link, List<int> removedIds)
		{
			if (removedIds.Contains(link))
			{
				link = -1;
			}
			else
			{
				var currentLink = link;
				var count = removedIds.Where(x => x < currentLink).Count();
				link -= count;
			}
		}
	}

	public class AdventureTextComponent : AdventureComponent
	{
		[Title("Descriptive/Background Text")]
		public string title;

		[TextArea(15, 20), PropertyOrder(0)]
		public string text;
	}

	public class AdventureNarrationComponent : AdventureComponent
	{
		[TextArea(15, 20), PropertyOrder(0), Title("Narration Text")]
		public string text;
	}

	public class AdventureBranchingComponent : AdventureComponent
	{
		[Title("Branching Action Paths"), ListDrawerSettings(CustomAddFunction = "AddPath")]
		public ObservableCollection<AdventurePathComponent> paths;

		public AdventureBranchingComponent()
		{
			paths = new ObservableCollection<AdventurePathComponent>();
			paths.CollectionChanged += EncounterEditorWindow.UpdateIDs;
		}

		public override void UpdateComponentID(ref int nextID, List<int> removedIds = null)
		{
			base.UpdateComponentID(ref nextID, removedIds);
			foreach (var path in paths)
			{
				path.UpdateComponentID(ref nextID, removedIds);
			}
		}

		private void AddPath()
		{
			paths.Add(new AdventurePathComponent());
		}
	}

	public class AdventurePathComponent : AdventureComponent
	{
		public ObservableCollection<IAdventureComponent> components;

		public AdventurePathComponent()
		{
			components = new ObservableCollection<IAdventureComponent>();
			components.CollectionChanged += EncounterEditorWindow.UpdateIDs;
		}

		public override void UpdateComponentID(ref int nextID, List<int> removedIds = null)
		{
			base.UpdateComponentID(ref nextID, removedIds);
			foreach (var component in components)
			{
				component.UpdateComponentID(ref nextID, removedIds);
			}
		}
	}

	public class AdventureSkillCheckComponent : AdventureComponent
	{
		//skill check type
		//difficulty
		[GUIColor("@successLink > -1 ? Color.green : Color.red")]
		public int successLink = -1;
		[GUIColor("@failureLink > -1 ? Color.green : Color.red")]
		public int failureLink = -1;

		public override void UpdateComponentID(ref int nextID, List<int> removedIds = null)
		{
			base.UpdateComponentID(ref nextID, removedIds);
			if (removedIds != null)
			{
				UpdateLinkID(ref successLink, removedIds);
				UpdateLinkID(ref failureLink, removedIds);
			}
		}
	}
}
