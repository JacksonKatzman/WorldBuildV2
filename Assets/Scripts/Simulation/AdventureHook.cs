using Game.Enums;
using Game.Incidents;
using Game.Terrain;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
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
		[PropertyOrder(-10)]
		public string encounterTitle;
		[PropertyOrder(-9)]
		public EncounterLocationType encounterLocationType;

		[ValueDropdown("GetEncounterTypes", IsUniqueList = true, DropdownTitle = "Encounter Types"), PropertyOrder(-8)]
		public List<EncounterType> encounterTypes;

		[ValueDropdown("GetBiomeTerrainTypes", IsUniqueList = true, DropdownTitle = "Allowed Biomes"), PropertyOrder(-7)]
		public List<BiomeTerrainType> allowedBiomes;

		[ListDrawerSettings(HideAddButton = true), PropertyOrder(0)]
		public List<IAdventureContextCriteria> contextCriterium;

		[TextArea, PropertyOrder(0)]
		public string encounterSummary;
		[PropertyOrder(0)]
		public List<AdventureEncounterPath> encounterPaths;

		public AdventureEncounter()
		{
			encounterTypes = new List<EncounterType>();
			allowedBiomes = new List<BiomeTerrainType>();
			encounterPaths = new List<AdventureEncounterPath>();
			contextCriterium = new List<IAdventureContextCriteria>();
		}

		private void UpdateContainerIDs()
		{
			for(int i = 0; i < contextCriterium.Count; i++)
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
			contextCriterium.Add(new AdventureContextCriteria(typeof(Person)));
			UpdateContainerIDs();
		}
	}

	public class AdventureEncounterPath
	{
		public string pathTitle;
		[TextArea]
		public string pathSummary;
		[TextArea]
		public string pathInformation;
	}

	public interface IAdventureContextCriteria 
	{
		public Type ContextType { get; set; }
		public string ContextID { get; set; }
	}

	[Serializable, HideReferenceObjectPicker]
	public class AdventureContextCriteria : IAdventureContextCriteria
	{
		public Type ContextType { get; set; }

		public string ContextID
		{
			get { return contextID; }
			set { contextID = value; }
		}

		[SerializeField, ReadOnly, HorizontalGroup, PropertyOrder(-1)]
		private string contextTypeName;
		[SerializeField, ReadOnly, HorizontalGroup, PropertyOrder(-1)]
		private string contextID;

		public AdventureContextCriteria(Type contextType)
		{
			ContextType = contextType;
			contextTypeName = contextType.Name;
		}
	}

	[Serializable, HideReferenceObjectPicker]
	public class MonsterCriteria : AdventureContextCriteria
	{
		public bool isLegendary;
		public bool isLandDwelling;

		[ValueDropdown("GetCreatureSizes", IsUniqueList = true, DropdownTitle = "Allowed Sizes")]
		public List<CreatureSize> allowedSizes;
		[ValueDropdown("GetCreatureTypes", IsUniqueList = true, DropdownTitle = "Allowed Types")]
		public List<CreatureType> allowedTypes;
		[ValueDropdown("GetCreatureAlignments", IsUniqueList = true, DropdownTitle = "Allowed Alignments")]
		public List<CreatureAlignment> allowedAlignments;

		public MonsterCriteria() : base(typeof(Monster))
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
