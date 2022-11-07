using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public enum LocationFindMethod { Random_Unclaimed, Random_Empty, Within_Faction }
	public class LocationActionField : ContextualIncidentActionField<Location>
	{
		protected override bool ShowStandardCriteria => false;
		[ShowInInspector, PropertyOrder(-1), ShowIf("@this.Method == ActionFieldRetrievalMethod.Criteria")]
		public LocationFindMethod LocationFindMethod { get; set; }
		[ShowIf("@this.ShowFactionBasedProperties")]
		public int minDistanceFromCities;
		private bool ShowFactionBasedProperties => LocationFindMethod == LocationFindMethod.Within_Faction;
		public LocationActionField() : base()
		{
		}

		public LocationActionField(Type parentType) : base(parentType) { }

		public override bool CalculateField(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			if (Method == ActionFieldRetrievalMethod.Criteria)
			{
				return FindNewLocation(context);
			}
			else if (Method == ActionFieldRetrievalMethod.From_Previous)
			{
				delayedValue = RetrieveFieldFromPrevious(context, delayedCalculateAction);
			}
			else
			{
				value = new Location(FindRandom());
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
				index = FindWithinFaction(context);
			}
			if (index != -1)
			{
				value = new Location(index);
				return true;
			}
			else
			{
				OutputLogger.LogWarning("FindLocationAction failed to find location with method: " + LocationFindMethod);
				return false;
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

		private int FindWithinFaction(IIncidentContext context)
		{
			if(!context.ContextType.IsAssignableFrom(typeof(IFactionAffiliated)))
			{
				return -1;
			}
			var faction = ((IFactionAffiliated)context).AffiliatedFaction;

			List<int> possibleIndices = new List<int>();
			var cityTiles = SimulationUtilities.GetCellsWithCities();
			foreach (var index in faction.ControlledTileIndices)
			{
				var cell = SimulationManager.Instance.HexGrid.cells[index];
				var valid = true;
				foreach (var cityIndex in cityTiles)
				{
					var cityCell = SimulationManager.Instance.HexGrid.cells[cityIndex];
					if (cell.coordinates.DistanceTo(cityCell.coordinates) < minDistanceFromCities)
					{
						valid = false;
						break;
					}
				}
				if (valid)
				{
					possibleIndices.Add(index);
				}
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
	}
	/*
	public class LocationActionField : IIncidentActionField
	{
		public ActionFieldRetrievalMethod Method { get; set; }
		public LocationFindMethod LocationFindMethod { get; set; }

		public int ActionFieldID { get; set; }

		[ShowInInspector]
		public string ActionFieldIDString => "{" + ActionFieldID + "}";

		public string NameID { get; set; }

		public Type ContextType => typeof(Location);

		private Location value;

		public bool CalculateField(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			if (Method == ActionFieldRetrievalMethod.Criteria)
			{
				return FindNewLocation(context);
			}
			else if (Method == ActionFieldRetrievalMethod.From_Previous)
			{
				delayedValue = RetrieveFieldFromPrevious(context, delayedCalculateAction);
			}
			else
			{
				value = RetrieveFieldAtRandom(context);
			}
		}

		public IIncidentContext GetFieldValue()
		{
			return value;
		}

		public Location GetTypedFieldValue()
		{
			return value;
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
				index = FindWithinFaction();
			}
			if (index != -1)
			{
				value = new Location(index);
				return true;
			}
			else
			{
				OutputLogger.LogWarning("FindLocationAction failed to find location with method: " + LocationFindMethod);
				return false;
			}
		}

		private IIncidentActionField RetrieveFieldFromPrevious(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			return delayedCalculateAction.Invoke(previousFieldID);
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

		private int FindWithinFaction()
		{
			if (faction.GetTypedFieldValue() == null)
			{
				return -1;
			}

			List<int> possibleIndices = new List<int>();
			var factionField = faction.GetTypedFieldValue();
			var cityTiles = SimulationUtilities.GetCellsWithCities();
			foreach (var index in factionField.ControlledTileIndices)
			{
				var cell = SimulationManager.Instance.HexGrid.cells[index];
				var valid = true;
				foreach (var cityIndex in cityTiles)
				{
					var cityCell = SimulationManager.Instance.HexGrid.cells[cityIndex];
					if (cell.coordinates.DistanceTo(cityCell.coordinates) < minDistanceFromCities)
					{
						valid = false;
						break;
					}
				}
				if (valid)
				{
					possibleIndices.Add(index);
				}
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
	}
	*/
}