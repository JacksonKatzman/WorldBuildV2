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

namespace Game.Incidents
{
	[Serializable]
	public class Faction : IncidentContext, IFactionAffiliated, IAlignmentAffiliated
	{
		public Faction AffiliatedFaction => this;
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
		public int InfluenceForNextTile => ControlledTiles * 2 + 1;
		[ES3Serializable]
		public List<City> Cities { get; set; }
		public City Capitol => Cities.Count > 0? Cities[0] : null;
		virtual public int NumCities => Cities.Count;
		public int PoliticalPriority { get; set; }
		public int EconomicPriority { get; set; }
		public int ReligiousPriority { get; set; }
		public int MilitaryPriority { get; set; }
		public int LawfulChaoticAlignmentAxis { get; set; }
		public int GoodEvilAlignmentAxis { get; set; }
		public List<IIncidentContext> FactionsWithinInteractionRange => GetFactionsWithinInteractionRange();
		public List<IIncidentContext> FactionsAtWarWith { get; set; }

		public bool AtWar => FactionsAtWarWith.Count > 0;
		public bool CouldMakePeace => FactionsAtWarWith.Where(x => FactionRelations[x] >= 0).ToList().Count >= 1;
		virtual public bool CanExpandTerritory => true;
		virtual public bool CanTakeMilitaryAction => true;
		public Organization Government { get; set; }
		public Race MajorityRace => Government.Leader.Race;

		[HideInInspector]
		public List<int> ControlledTileIndices { get; set; }

		public NamingTheme namingTheme;

		public Faction() : base()
		{
			EventManager.Instance.AddEventHandler<RemoveContextEvent>(OnRemoveContextEvent);
		}

		public Faction(int startingTiles, int startingPopulation, Race startingMajorityRace) : this()
		{
			AttemptExpandBorder(startingTiles);
			FactionRelations = new Dictionary<IIncidentContext, int>();
			Cities = new List<City>();
			FactionsAtWarWith = new List<IIncidentContext>();
			
			namingTheme = new NamingTheme(startingMajorityRace.racePreset.namingTheme);
			Name = namingTheme.GenerateFactionName();

			CreateStartingCity(startingPopulation);
			CreateStartingGovernment(startingMajorityRace);

			PoliticalPriority = SimRandom.RandomRange(1, 4);
			ReligiousPriority = SimRandom.RandomRange(1, 4);
			EconomicPriority = SimRandom.RandomRange(1, 4);
			MilitaryPriority = SimRandom.RandomRange(1, 4);
		}

		public Faction(int population, int influence, int wealth, int politicalPriority, int economicPriority, int religiousPriority, int militaryPriority, Race race, int startingTiles = 1) : this(startingTiles, population, race)
		{
			Influence = influence;
			Wealth = wealth;
			PoliticalPriority = politicalPriority;
			EconomicPriority = economicPriority;
			ReligiousPriority = religiousPriority;
			MilitaryPriority = militaryPriority;
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

			if (CheckDestroyed())
			{
				Die();
			}
		}
		override public void UpdateContext()
		{
			UpdateWealth();
			UpdatePopulation();
			UpdateInfluence();
			UpdatePERMS();
			UpdateNumIncidents();
		}

		override public void Die()
		{
			EventManager.Instance.RemoveEventHandler<RemoveContextEvent>(OnRemoveContextEvent);
			EventManager.Instance.Dispatch(new RemoveContextEvent(this));
			Government.Die();
		}

		public void CreateStartingCity(int startingPopulation)
		{
			var cells = SimulationUtilities.GetCitylessCellsFromList(ControlledTileIndices);
			var location = new Location(SimRandom.RandomEntryFromList(cells));
			var city = new City(this, location, startingPopulation, 0);
			Cities.Add(city);
			SimulationManager.Instance.world.AddContext(city);
		}

		public void CreateStartingGovernment(Race majorityStartingRace)
		{
			Government = new Organization(this, majorityStartingRace, Enums.OrganizationType.POLITICAL);
			EventManager.Instance.Dispatch(new AddContextEvent(Government));
		}

		public bool AttemptExpandBorder(int numTimes)
		{
			HexCellPriorityQueue searchFrontier = new HexCellPriorityQueue();
			int searchFrontierPhase = 1;
			int size = 0;

			if (ControlledTileIndices == null)
			{
				ControlledTileIndices = new List<int>();
			}
			if (ControlledTileIndices.Count == 0)
			{
				if (SimulationUtilities.GetRandomUnclaimedCellIndex(out var index))
				{
					ControlledTileIndices.Add(index);
					size++;
				}
				else
				{
					OutputLogger.LogError("Couldn't find free tile to create faction on!");
					return false;
				}
			}

			HexCell firstCell = SimulationManager.Instance.HexGrid.GetCell(ControlledTileIndices[0]);
			firstCell.SearchPhase = searchFrontierPhase;
			firstCell.Distance = 0;
			firstCell.SearchHeuristic = 0;
			searchFrontier.Enqueue(firstCell);
			var center = firstCell.coordinates;

			while (size < numTimes && searchFrontier.Count > 0)
			{
				HexCell current = searchFrontier.Dequeue();
				if (SimulationUtilities.IsCellIndexUnclaimed(current.Index))
				{
					ControlledTileIndices.Add(current.Index);
					size++;
				}

				for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
				{
					HexCell neighbor = current.GetNeighbor(d);
					if (neighbor && neighbor.SearchPhase < searchFrontierPhase)
					{
						neighbor.SearchPhase = searchFrontierPhase;
						neighbor.Distance = neighbor.coordinates.DistanceTo(center);
						neighbor.SearchHeuristic = SimRandom.RandomFloat01() < 0.25f ? 1 : 0;
						searchFrontier.Enqueue(neighbor);
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
			if(gameEvent.context.ContextType == typeof(Faction))
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
			if(gameEvent.context.ContextType == typeof(City))
			{
				if(Cities != null && Cities.Contains((City)gameEvent.context))
				{
					Cities.Remove((City)gameEvent.context);
				}
			}
		}

		private List<IIncidentContext> GetFactionsWithinInteractionRange()
		{
			var world = SimulationManager.Instance.world;
			var range = ((ControlledTiles / 6) + PoliticalPriority) * 5;
			var result = new List<IIncidentContext>();

			if(Capitol == null)
			{
				return result;
			}

			foreach(var f in world.CurrentContexts[typeof(Faction)])
			{
				var faction = (Faction)f;

				if (faction != this && faction.Capitol != null && Capitol.GetDistanceBetweenLocations(faction.Capitol) <= range)
				{
					result.Add(faction);
				}
			}

			return result;
		}

		private void UpdateInfluence()
		{
			Influence += 3;
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
				MilitaryPower += MilitaryPriority / 2;
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
	}
}