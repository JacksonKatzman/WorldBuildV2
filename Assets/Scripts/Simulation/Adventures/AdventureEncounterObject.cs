using Game.Enums;
using Game.Incidents;
using Game.Terrain;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
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
	}
}
