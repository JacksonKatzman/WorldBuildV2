using Game.Debug;
using Game.Enums;
using Game.Generators.Items;
using Game.Generators.Names;
using Game.Incidents;
using Game.Simulation;
using Game.Terrain;
using Game.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using HexDirection = Game.Terrain.HexDirection;

namespace Game.Incidents
{
	[Serializable]
	public class Faction : IncidentContext, IFactionAffiliated, IAlignmentAffiliated, IInventoryAffiliated
	{
		public Faction AffiliatedFaction
		{
			get => this;
			set
			{
				OutputLogger.LogWarning("You cannot set the affiliated faction of a faction.");
			}
		}
		public Type FactionType => ContextType;
		virtual public int Population
		{
			get
			{
				return Cities == null ? 0 : Cities.Sum(x => x.Population);
			}
			set
			{
				OutputLogger.LogWarning("You cannot directly set a factions population.");
			}
		}
		public int Influence { get; set; }
		public int Wealth { get; set; }
		public int MilitaryPower { get; set; }
		public Dictionary<IIncidentContext, int> FactionRelations { get; set; }
		virtual public int ControlledTiles => ControlledTileIndices.Count;
		public int InfluenceForNextTile => (ControlledTiles * 2)/3 + 1;
		[ES3Serializable]
		public List<City> Cities { get; set; }
		public City Capitol => Cities.Count > 0? Cities[0] : null;
		virtual public int NumCities => Cities.Count;

		//these need to interact with the dictionary or have all of their uses converted over to use the dict directly.
		public int PoliticalPriority
		{
			get { return Priorities[OrganizationType.POLITICAL]; }
			set { Priorities[OrganizationType.POLITICAL] = value; }
		}
		public int EconomicPriority
		{
			get { return Priorities[OrganizationType.ECONOMIC]; }
			set { Priorities[OrganizationType.ECONOMIC] = value; }
		}
		public int ReligiousPriority
		{
			get { return Priorities[OrganizationType.RELIGIOUS]; }
			set { Priorities[OrganizationType.RELIGIOUS] = value; }
		}
		public int MilitaryPriority
		{
			get { return Priorities[OrganizationType.MILITARY]; }
			set { Priorities[OrganizationType.MILITARY] = value; }
		}
		public Dictionary<OrganizationType, int> Priorities { get; set; }
		public OrganizationType PriorityAlignment => GetHighestPriority();
		public int LawfulChaoticAlignmentAxis { get; set; }
		public int GoodEvilAlignmentAxis { get; set; }
		public List<IIncidentContext> FactionsWithinInteractionRange => GetFactionsWithinInteractionRange();
		public List<IIncidentContext> FactionsAtWarWith { get; set; }

		public bool AtWar => FactionsAtWarWith.Count > 0;
		public bool CouldMakePeace => FactionsAtWarWith.Where(x => FactionRelations[x] >= 0).ToList().Count >= 1;
		virtual public bool CanExpandTerritory => true;
		virtual public bool CanTakeMilitaryAction => true;
		public Organization Government { get; set; }
		public Inventory CurrentInventory
		{
			get
			{
				if(inventory == null)
				{
					inventory = new FactionInventory(this);
				}
				return inventory;
			}
		}
		private FactionInventory inventory;

		public Race MajorityRace => Government.Leader.AffiliatedRace;
		virtual public bool IsSpecialFaction => false;

		[HideInInspector]
		public List<int> ControlledTileIndices { get; set; }

		public NamingTheme namingTheme;

		public Faction() : base()
		{
			EventManager.Instance.AddEventHandler<RemoveContextEvent>(OnRemoveContextEvent);
		}

		public Faction(int startingTiles, int startingPopulation, Race startingMajorityRace, Character creator = null) : this()
		{
			FactionRelations = new Dictionary<IIncidentContext, int>();
			Cities = new List<City>();
			FactionsAtWarWith = new List<IIncidentContext>();

			Priorities = new Dictionary<OrganizationType, int>();
			Priorities[OrganizationType.POLITICAL] = SimRandom.RandomRange(1, 4);
			Priorities[OrganizationType.ECONOMIC] = SimRandom.RandomRange(1, 4);
			Priorities[OrganizationType.RELIGIOUS] = SimRandom.RandomRange(1, 4);
			Priorities[OrganizationType.MILITARY] = SimRandom.RandomRange(1, 4);

			namingTheme = new NamingTheme(startingMajorityRace.racePreset.namingTheme);
			Name = namingTheme.GenerateFactionName();

			ClaimFirstCell(startingPopulation);
			if(startingTiles > 1)
			{
				AttemptExpandBorder(startingTiles - 1);
			}

			CreateStartingGovernment(startingMajorityRace, creator);
		}

		public Faction(int population, int influence, int wealth, int politicalPriority, int economicPriority, int religiousPriority, int militaryPriority, Race race, int startingTiles = 1, Character creator = null) : this(startingTiles, population, race, creator)
		{
			Influence = influence;
			Wealth = wealth;
			Priorities = new Dictionary<OrganizationType, int>();
			Priorities[OrganizationType.POLITICAL] = politicalPriority;
			Priorities[OrganizationType.ECONOMIC] = economicPriority;
			Priorities[OrganizationType.RELIGIOUS] = religiousPriority;
			Priorities[OrganizationType.MILITARY] = militaryPriority;
		}

		public Faction(Race race)
		{
			namingTheme = new NamingTheme(race.racePreset.namingTheme);
			Name = namingTheme.GenerateFactionName();
			CreateStartingGovernment(race);
		}

		override public void DeployContext()
		{
			if (NumIncidents > 0)
			{
				IncidentService.Instance.PerformIncidents((Faction)this);
			}

			CheckForDeath();
		}
		override public void UpdateContext()
		{
			UpdateWealth();
			UpdatePopulation();
			UpdateInfluence();
			UpdatePERMS();
			UpdateNumIncidents();
		}

		public override void CheckForDeath()
		{
			if (CheckDestroyed())
			{
				Die();
			}
		}

		override public void Die()
		{
			EventManager.Instance.RemoveEventHandler<RemoveContextEvent>(OnRemoveContextEvent);
			EventManager.Instance.Dispatch(new RemoveContextEvent(this, GetType()));
			Government.Die();
			IncidentService.Instance.ReportStaticIncident(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>{0} is wiped out!", new List<IIncidentContext>() { this }, true);
		}

		public void CreateStartingCity(int startingPopulation)
		{
			var cells = SimulationUtilities.GetCitylessCellsFromList(ControlledTileIndices);
			var location = new Location(SimRandom.RandomEntryFromList(cells));
			var city = new City(this, location, startingPopulation, 0);
			Cities.Add(city);
			EventManager.Instance.Dispatch(new AddContextEvent(city, true));
		}

		public void CreateStartingGovernment(Race majorityStartingRace, Character creator = null)
		{
			Government = new Organization(this, majorityStartingRace, Enums.OrganizationType.POLITICAL, creator);
			EventManager.Instance.Dispatch(new AddContextEvent(Government, typeof(Organization), true));
		}

		public bool ClaimFirstCell(int startingPopulation)
		{
			if (ControlledTileIndices == null)
			{
				ControlledTileIndices = new List<int>();
			}
			if (ControlledTileIndices.Count == 0)
			{
				//var possibleTiles = SimulationUtilities.GetAllCellsWithDistanceFromCity(10, 15);
				var possibleTiles = SimulationUtilities.SecondTry(8);
				if (possibleTiles.Count > 0)
				{
					var tile = SimRandom.RandomEntryFromList(possibleTiles);
					ControlledTileIndices.Add(tile.Index);
					CreateStartingCity(startingPopulation);
					return true;
				}
				else if (SimulationUtilities.GetRandomUnclaimedCellIndex(out var index))
				{
					ControlledTileIndices.Add(index);
					CreateStartingCity(startingPopulation);
					return true;
				}
				else
				{
					OutputLogger.LogError("Couldn't find free tile to create faction on!");
					return false;
				}
			}
			return false;
		}

		public bool AttemptExpandBorder(int numTimes)
		{
			//return true;
			HexCellPriorityQueue searchFrontier = new HexCellPriorityQueue();
			int searchFrontierPhase = 1;
			int size = 0;

			HexCell firstCell = SimulationManager.Instance.HexGrid.GetCell(ControlledTileIndices[0]);
			firstCell.SearchPhase = searchFrontierPhase;
			firstCell.Distance = 0;
			firstCell.SearchHeuristic = 0;
			searchFrontier.Enqueue(firstCell);
			var center = firstCell.coordinates;

			while (size < numTimes && searchFrontier.Count > 0)
			{
				HexCell current = searchFrontier.Dequeue();
				var claimedCells = SimulationUtilities.GetClaimedCells();
				var outsideBorder = SimulationUtilities.FindBorderOutsideFaction(this);
				if (!claimedCells.Contains(current.Index))
				{
					ControlledTileIndices.Add(current.Index);
					size++;
					if(size >= numTimes)
					{
						break;
					}
				}

				for (Terrain.HexDirection d = Terrain.HexDirection.NE; d <= Terrain.HexDirection.NW; d++)
				{
					HexCell neighbor = current.GetNeighbor(d);
					if (neighbor && neighbor.SearchPhase < searchFrontierPhase)
					{
						neighbor.SearchPhase = searchFrontierPhase;
						neighbor.Distance = neighbor.coordinates.DistanceTo(center);
						neighbor.SearchHeuristic = SimRandom.RandomFloat01() < 0.25f ? 1 : 0;
						var weControl = ControlledTileIndices.Contains(neighbor.Index);
						if((outsideBorder.Contains(neighbor.Index) || ControlledTileIndices.Contains(neighbor.Index)) )
						{
							if(!SimulationUtilities.GetClaimedCellsNotClaimedByFaction(this).Contains(neighbor.Index) && !neighbor.IsUnderwater)
							{
								searchFrontier.Enqueue(neighbor);
							}
						}
					}
				}
			}

			SimulationManager.Instance.HexGrid.ResetSearchPhases();

			return size != 0;
		}

		public override void LoadContextProperties()
		{
			Government = SaveUtilities.ConvertIDToContext<Organization>(contextIDLoadBuffers["Government"][0]);
			Cities = SaveUtilities.ConvertIDsToContexts<City>(contextIDLoadBuffers["Cities"]);
			FactionsAtWarWith = SaveUtilities.ConvertIDsToContexts<IIncidentContext>(contextIDLoadBuffers["FactionsAtWarWith"]);
			var relationKeys = SaveUtilities.ConvertIDsToContexts<Faction>(contextIDLoadBuffers["FactionRelationKeys"]);
			var relationValues = contextIDLoadBuffers["FactionRelationValues"];

			if(FactionRelations == null)
			{
				FactionRelations = new Dictionary<IIncidentContext, int>();
			}

			for(int i = 0; i < relationKeys.Count; i++)
			{
				FactionRelations.Add(relationKeys[i], relationValues[i]);
			}

			contextIDLoadBuffers.Clear();
		}

		private void OnRemoveContextEvent(RemoveContextEvent gameEvent)
		{
			if(gameEvent.context.GetType() == typeof(Faction))
			{
				//remove factions from collections
				if(FactionRelations.Keys.Contains(gameEvent.context))
				{
					FactionRelations.Remove(gameEvent.context);
				}
				if(FactionsAtWarWith.Contains(gameEvent.context))
				{
					FactionsAtWarWith.Remove(gameEvent.context);
				}
			}
			if(gameEvent.context.GetType() == typeof(City))
			{
				if(Cities != null && Cities.Contains((City)gameEvent.context))
				{
					Cities.Remove((City)gameEvent.context);
					if (CheckDestroyed())
					{
						Die();
					}
				}
			}
		}

		private List<IIncidentContext> GetFactionsWithinInteractionRange()
		{
			var world = SimulationManager.Instance.world;
			var range = ((ControlledTiles / 6) + Priorities[OrganizationType.POLITICAL]) * 5;
			var result = new List<IIncidentContext>();

			if(Capitol == null)
			{
				return result;
			}

			foreach(var f in ContextDictionaryProvider.CurrentContexts[typeof(Faction)])
			{
				var faction = (Faction)f;

				if (faction != this && faction.Capitol != null && Capitol.GetDistanceBetweenLocations(faction.Capitol) <= range)
				{
					result.Add(faction);
				}
			}

			return result;
		}

		public void ClaimTerritoryBetweenCities()
		{
			var faction = this;
			var cityHexes = new List<HexCell>();
			foreach (var city in faction.Cities)
			{
				cityHexes.Add(city.CurrentLocation.GetHexCell());
			}

			var centroid = SimulationUtilities.GetCentroid(cityHexes);

			var totalDistance = 0;
			foreach (var city in faction.Cities)
			{
				totalDistance += city.CurrentLocation.GetHexCell().coordinates.DistanceTo(centroid.coordinates);
			}
			var averageDistance = totalDistance / faction.Cities.Count;

			var hexes = SimulationUtilities.GetAllCellsInRange(centroid, averageDistance);
			var claimedCells = SimulationUtilities.GetClaimedCells();
			foreach (var hex in hexes)
			{
				if (!claimedCells.Contains(hex.Index))
				{
					faction.ControlledTileIndices.Add(hex.Index);
				}
			}
		}

		private void UpdateInfluence()
		{
			Influence += (5 + PoliticalPriority/3);
		}

		private void UpdateWealth()
		{
			foreach (var city in Cities)
			{
				Wealth += city.GenerateWealth();
			}
		}

		private void UpdatePopulation()
		{
			foreach(var city in Cities)
			{
				city.UpdatePopulation();
			}

			if(MilitaryPower < (Population/10))
			{
				MilitaryPower += Priorities[OrganizationType.MILITARY] / 2;
			}
		}

		private void UpdatePERMS()
		{

		}

		private void UpdateNumIncidents()
		{
			NumIncidents = 1;
		}

		private bool CheckDestroyed()
		{
			return NumCities <= 0;
		}

		private OrganizationType GetHighestPriority()
		{
			return Priorities.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
		}
	}
}