using Game.Simulation;
using Game.Terrain;
using Game.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Game.Incidents.HexCollection;

namespace Game.Incidents
{
	public class FindLocationNearLocationAction : GenericIncidentAction
	{
		//overload the verify to grab/create a location and jam it into the second field like getorcreate does
		public InterfacedIncidentActionFieldContainer<ILocationAffiliated> startingLocation;

		[ValueDropdown("GetFilteredTypeList")]
		public Type typeToSearchFor;

		public int maxDistance;

		[ValueDropdown("GetBiomeTerrainTypes", IsUniqueList = true, DropdownTitle = "Allowed Sizes")]
		public List<BiomeTerrainType> allowedBiomes = new List<BiomeTerrainType>();

		//Deprecated
		[HideInInspector]
		public bool isMountainous;

		[ShowIf("@this.ShowHexCollectionOptions")]
		public HexCollectionType hexCollectionType;

		private bool ShowHexCollectionOptions => typeToSearchFor == typeof(HexCollection);

		[HideInInspector]
		public LocationActionField locationNearLocation;

		[ReadOnly, ShowInInspector]
		public string ResultID => locationNearLocation?.ActionFieldIDString;

		public override bool VerifyAction(IIncidentContext context)
		{
			locationNearLocation.AllowNull = true;
			var baseVerified = base.VerifyAction(context);
			if(!baseVerified)
			{
				return false;
			}

			var possibilities = new List<IIncidentContext>();
			//var possibilities = ContextDictionaryProvider.CurrentContexts[typeToSearchFor].Where(x => ((ILocationAffiliated)x).GetDistanceBetweenLocations(startingLocation.GetTypedFieldValue()) <= maxDistance).ToList();
			foreach(var c in ContextDictionaryProvider.CurrentContexts[typeToSearchFor])
			{
				var distance = ((ILocationAffiliated)c).GetDistanceBetweenLocations(startingLocation.GetTypedFieldValue());
				if(distance <= maxDistance)
				{
					possibilities.Add(c);
				}
			}
			possibilities = possibilities.Where(x => allowedBiomes.Count > 0 ? allowedBiomes.Contains(((ILocationAffiliated)x).CurrentLocation.AffiliatedTerrainType) : true).ToList();
			if(typeToSearchFor == typeof(HexCollection))
			{
				possibilities = possibilities.Where(x => ((HexCollection)x).CollectionType == hexCollectionType).ToList();
			}
			if (possibilities.Count > 0)
			{
				var locationData = SimRandom.RandomEntryFromList(possibilities) as ILocationAffiliated;
				locationNearLocation.value = locationData.CurrentLocation;
				return true && baseVerified;
			}
			else
			{
				return false;
			}
		}

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			
		}

		protected IEnumerable<Type> GetFilteredTypeList()
		{
			var q = typeof(IIncidentContext).Assembly.GetTypes()
				.Where(x => !x.IsAbstract)                                          // Excludes BaseClass
				.Where(x => !x.IsGenericTypeDefinition)                             // Excludes Generics
				.Where(x => typeof(ILocationAffiliated).IsAssignableFrom(x))
				.Where(x => typeof(IIncidentContext).IsAssignableFrom(x));

			return q;
		}

		private IEnumerable<BiomeTerrainType> GetBiomeTerrainTypes()
		{
			return Enum.GetValues(typeof(BiomeTerrainType)).Cast<BiomeTerrainType>();
		}
	}
}