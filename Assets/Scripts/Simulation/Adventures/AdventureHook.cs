using Game.Enums;
using Game.Incidents;
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

	[HideReferenceObjectPicker]
	public class AdventureEncounterAct
	{
		public string actTitle;
		public AdventureEncounterAct()
		{
			paths = new List<AdventureEncounterPath>();
		}

		[ListDrawerSettings(CustomAddFunction = "AddPath")]
		public List<AdventureEncounterPath> paths;

		private void AddPath()
		{
			paths.Add(new AdventureEncounterPath());
		}
	}

	[HideReferenceObjectPicker]
	public class AdventureEncounterPath
	{
		public string pathTitle;
		[TextArea(10,15), FoldoutGroup("Summary")]
		public string pathSummary;
		[TextArea(15,20), FoldoutGroup("Full Description")]
		public string pathInformation;
	}

	public interface IAdventureContextCriteria 
	{
		public Type ContextType { get; }
		public string ContextID { get; set; }
		public IIncidentContext Context { get; set; }
		public bool IsHistorical { get; }
	}

	[Serializable, HideReferenceObjectPicker]
	public class AdventureContextCriteria<T> : IAdventureContextCriteria where T : IIncidentContext
	{
		public Type ContextType => typeof(T);

		public string ContextID
		{
			get { return contextID; }
			set { contextID = value; }
		}

		public IIncidentContext Context { get; set; }
		public bool IsHistorical => historical;

		[SerializeField, ReadOnly, HorizontalGroup(LabelWidth = 120), PropertyOrder(-1)]
		private string contextTypeName;
		[SerializeField, ReadOnly, HorizontalGroup, PropertyOrder(-1)]
		private string contextID;
		[SerializeField, HorizontalGroup, PropertyOrder(-1)]
		public bool historical;

		public AdventureContextCriteria()
		{
			contextTypeName = ContextType.Name;
		}
	}

	[Serializable, HideReferenceObjectPicker]
	public class MonsterCriteria : AdventureContextCriteria<Monster>
	{
		public bool isLegendary;
		public bool isLandDwelling;

		[ValueDropdown("GetCreatureSizes", IsUniqueList = true, DropdownTitle = "Allowed Sizes")]
		public List<CreatureSize> allowedSizes;
		[ValueDropdown("GetCreatureTypes", IsUniqueList = true, DropdownTitle = "Allowed Types")]
		public List<CreatureType> allowedTypes;
		[ValueDropdown("GetCreatureAlignments", IsUniqueList = true, DropdownTitle = "Allowed Alignments")]
		public List<CreatureAlignment> allowedAlignments;

		public MonsterCriteria() : base()
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
