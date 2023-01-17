using Game.Enums;
using Game.Incidents;
using Game.Terrain;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Simulation
{
	public class AdventureHook
	{
		public string name;
		public Location location;
		//adventure origin
		//general description*
		//possible objectives*
		//battle map type?
		//doodad prefab
		//included encounter


		/*
		 * Example description:
		 * There is a [Point of Interest Type] nearby, located in [Location].
		 * It is inhabited by [Known Threats].
		 * (If historical sight) It is thought to be X years old, [more historical data].
		 * [Quest Giver] has tasked adventurers with [Objectives].
		 * [Reason for quest]
		 * [Reward for quest]
		 */
	}

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

	[CreateAssetMenu(fileName = nameof(AdventureEncounter), menuName = "ScriptableObjects/Adventures/" + nameof(AdventureEncounter), order = 1)]
	public class AdventureEncounter : SerializedScriptableObject
	{
		public EncounterLocationType encounterLocationType;

		[ValueDropdown("GetEncounterTypes", IsUniqueList = true, DropdownTitle = "Encounter Types")]
		public List<EncounterType> encounterTypes;

		[ValueDropdown("GetBiomeTerrainTypes", IsUniqueList = true, DropdownTitle = "Allowed Biomes")]
		public List<BiomeTerrainType> allowedBiomes;

		public bool containsPossibleThreat;

		[ShowIf("@this.containsPossibleThreat"), ListDrawerSettings(CustomAddFunction = "AddNewCriteriaItem", CustomRemoveIndexFunction = "RemoveCriteriaItem"), HideReferenceObjectPicker]
		public List<ThreatCriteriaContainer> possibleThreats;

		public AdventureEncounter()
		{
			encounterTypes = new List<EncounterType>();
			allowedBiomes = new List<BiomeTerrainType>();
			possibleThreats = new List<ThreatCriteriaContainer>();
		}

		private void AddNewCriteriaItem()
		{
			possibleThreats.Add(new ThreatCriteriaContainer());
			UpdateContainerIDs();
		}

		private void RemoveCriteriaItem(int index)
		{
			possibleThreats.RemoveAt(index);
			UpdateContainerIDs();
		}

		private void UpdateContainerIDs()
		{
			for(int i = 0; i < possibleThreats.Count; i++)
			{
				possibleThreats[i].ContainerID = i;
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
	}

	[HideReferenceObjectPicker]
	public class ThreatCriteriaContainer
	{
		[ReadOnly, ShowInInspector]
		public int ContainerID { get; set; }

		[ValueDropdown("GetThreatCriteriaTypes"), OnValueChanged("SetCriteriaType"), LabelText("Threat Type")]
		public Type threatCriteriaType;

		[Range(0.0f, 1.0f)]
		public float crPercentage;

		[ShowIf("@this.threatCriteriaType != null")]
		public IThreatCriteria threatCriteria;

		public ThreatCriteriaContainer() { }

		private IEnumerable<Type> GetThreatCriteriaTypes()
		{
			var q = typeof(IThreatCriteria).Assembly.GetTypes()
				.Where(x => !x.IsAbstract)                                          // Excludes BaseClass
				.Where(x => !x.IsGenericTypeDefinition)                             // Excludes Generics
				.Where(x => typeof(IThreatCriteria).IsAssignableFrom(x));           // Excludes classes not inheriting from IIncidentContext

			return q;
		}

		private void SetCriteriaType()
		{
			threatCriteria = (IThreatCriteria)Activator.CreateInstance(threatCriteriaType);
		}
	}

	public interface IThreatCriteria
	{
	}

	[SerializeField, HideReferenceObjectPicker]
	public class MonsterCriteria : IThreatCriteria
	{
		public bool isLegendary;
		public bool isLandDwelling;

		[ValueDropdown("GetCreatureSizes", IsUniqueList = true, DropdownTitle = "Allowed Sizes")]
		public List<CreatureSize> allowedSizes;
		[ValueDropdown("GetCreatureTypes", IsUniqueList = true, DropdownTitle = "Allowed Types")]
		public List<CreatureType> allowedTypes;
		[ValueDropdown("GetCreatureAlignments", IsUniqueList = true, DropdownTitle = "Allowed Alignments")]
		public List<CreatureAlignment> allowedAlignments;

		public MonsterCriteria()
		{
			allowedSizes = new List<CreatureSize>();
			allowedTypes = new List<CreatureType>();
			allowedAlignments = new List<CreatureAlignment>();
		}

		private IEnumerable<CreatureSize> GetCreatureSizes()
		{
			return Enum.GetValues(typeof(CreatureSize)).Cast<CreatureSize>();
		}

		private IEnumerable<CreatureType> GetCreatureTypes()
		{
			return Enum.GetValues(typeof(CreatureType)).Cast<CreatureType>();
		}

		private IEnumerable<CreatureAlignment> GetCreatureAlignments()
		{
			return Enum.GetValues(typeof(CreatureAlignment)).Cast<CreatureAlignment>();
		}
	}
}
