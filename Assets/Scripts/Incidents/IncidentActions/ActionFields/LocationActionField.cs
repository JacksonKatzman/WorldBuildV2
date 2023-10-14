using Game.Simulation;
using Game.Terrain;
using Game.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public enum LocationFindMethod { Random_Unclaimed, Random_Empty, Within_Faction }
	public enum FactionCellLocationMethod { Within, Border_Within, Border_Without, Border_Shared }
	public class LocationActionField : ContextualIncidentActionField<Location>
	{
		protected override bool ShowStandardCriteria => false;
		protected override bool ShowMethodChoice => true;
		[ShowInInspector, PropertyOrder(-2), ShowIf("@this.Method == ActionFieldRetrievalMethod.Criteria")]
		public LocationFindMethod LocationFindMethod { get; set; }

		[ShowInInspector, PropertyOrder(-1), ShowIf("@this.ShowFactionBasedProperties")]
		public FactionCellLocationMethod FactionCellLocationMethod { get; set; }

		[ShowIf("@this.ShowFactionBasedProperties")]
		public ContextualIncidentActionField<Faction> relatedFaction;

		[ShowIf("@this.ShowFactionBasedProperties")]
		public int minDistanceFromCities;
		private bool ShowFactionBasedProperties => LocationFindMethod == LocationFindMethod.Within_Faction;
		public LocationActionField() : base()
		{
			relatedFaction = new ContextualIncidentActionField<Faction>();
		}

		public LocationActionField(Type parentType) : base(parentType) { }

		public override bool CalculateField(IIncidentContext context)
		{
			if (Method == ActionFieldRetrievalMethod.Criteria)
			{
				return FindNewLocation(context);
			}
			else if (Method == ActionFieldRetrievalMethod.From_Previous)
			{
				delayedValue = RetrieveFieldFromPrevious(context);
				return delayedValue != null;
			}
			else
			{
				var id = FindRandom();
				AssignLocationValue(id);
			}
			return true;
		}
		private bool FindNewLocation(IIncidentContext context)
		{
			var index = -1;

			if (LocationFindMethod == LocationFindMethod.Random_Unclaimed)
			{
				index = FindRandomUnclaimed();
			}
			else if (LocationFindMethod == LocationFindMethod.Random_Empty)
			{
				index = FindRandomEmpty();
			}
			else
			{
				if (!relatedFaction.CalculateField(context))
				{
					return false;
				}
				index = FindFactionRelatedCell(context);
			}
			if (index != -1)
			{
				AssignLocationValue(index);
				return true;
			}
			else
			{
				OutputLogger.LogWarning("FindLocationAction failed to find location with method: " + LocationFindMethod);
				return false;
			}
		}

		private void AssignLocationValue(int id)
		{
			var matches = ContextDictionaryProvider.CurrentContexts[typeof(Location)].Where(x => ((Location)x).TileIndex == id).ToList();
			if (matches.Count > 0)
			{
				value = (Location)matches[0];
			}
			else
			{
				value = new Location(id);
				EventManager.Instance.Dispatch(new AddContextEvent(value));
			}
		}

		private int FindRandom()
		{
			return SimulationUtilities.GetRandomCellIndex();
		}

		private int FindRandomUnclaimed()
		{
			SimulationUtilities.GetRandomUnclaimedCellIndex(out var index);
			return index;
		}

		private int FindRandomEmpty()
		{
			SimulationUtilities.GetRandomEmptyCellIndex(out var index);
			return index;
		}

		private int FindFactionRelatedCell(IIncidentContext context)
		{
			var faction = relatedFaction.GetTypedFieldValue();
			List<int> possibleIndices;

			if(FactionCellLocationMethod == FactionCellLocationMethod.Within)
			{
				possibleIndices = SimulationUtilities.FindCitylessCellWithinFaction(faction, minDistanceFromCities);
			}
			else if(FactionCellLocationMethod == FactionCellLocationMethod.Border_Within)
			{
				possibleIndices = SimulationUtilities.FindCitylessBorderWithinFaction(faction);
			}
			else if(FactionCellLocationMethod == FactionCellLocationMethod.Border_Without)
			{
				possibleIndices = SimulationUtilities.FindBorderOutsideFaction(faction);
			}
			else
			{
				possibleIndices = SimulationUtilities.FindSharedBorderFaction(faction);
			}

			if (possibleIndices.Count > 0)
			{
				return possibleIndices[SimRandom.RandomRange(0, possibleIndices.Count)];
			}
			else
			{
				return -1;
			}
		}
#if UNITY_EDITOR
		private IEnumerable<string> GetFactionProperties()
		{
			var contextType = IncidentEditorWindow.ContextType;
			var properties = contextType.GetProperties().Where(x => x.PropertyType == typeof(Faction)).Select(x => x.Name).ToList();
			return properties;
		}
#endif
	}
}