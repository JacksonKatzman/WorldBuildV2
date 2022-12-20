using Game.Incidents;
using Game.Simulation;
using Game.Terrain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	[Serializable]
	public class Faction : IncidentContext, IFactionAffiliated
	{
		public Faction AffiliatedFaction => this;
		public Type FactionType => ContextType;
		public int Population
		{
			get
			{
				return (int)populationFloat;
			}
			set
			{
				populationFloat = (float)value;
			}
		}
		public int Influence { get; set; }
		public int Wealth { get; set; }
		public int MilitaryPower { get; set; }
		public Dictionary<IIncidentContext, int> FactionRelations { get; set; }
		public int ControlledTiles => ControlledTileIndices.Count;
		public int InfluenceForNextTile => ControlledTiles * 2 + 1;
		public List<City> Cities { get; set; }
		public City Capitol => Cities.Count > 0? Cities[0] : null;
		public int NumCities => Cities.Count;
		public int PoliticalPriority { get; set; }
		public int EconomicPriority { get; set; }
		public int ReligiousPriority { get; set; }
		public int MilitaryPriority { get; set; }
		public List<IIncidentContext> FactionsWithinInteractionRange => GetFactionsWithinInteractionRange();
		public List<IIncidentContext> FactionsAtWarWith { get; set; }

		public bool AtWar => FactionsAtWarWith.Count > 0;
		public bool CouldMakePeace => FactionsAtWarWith.Where(x => FactionRelations[x] >= 0).ToList().Count >= 1;
		virtual public bool CanExpandTerritory => true;
		virtual public bool CanTakeMilitaryAction => true;
		public Government Government { get; set; }

		[HideInInspector]
		public List<int> ControlledTileIndices { get; set; }

		private float populationFloat;

		public Faction() : base()
		{
			//need a smart generic way to go through our collections of contexts and remove a context
			//when we get sent a contextremovedevent
			EventManager.Instance.AddEventHandler<RemoveContextEvent>(OnRemoveContextEvent);
		}

		public Faction(int startingTiles) : this()
		{
			AttemptExpandBorder(startingTiles);
			FactionRelations = new Dictionary<IIncidentContext, int>();
			Cities = new List<City>();
			FactionsAtWarWith = new List<IIncidentContext>();
			CreateStartingCity();
			CreateStartingGovernment();

			populationFloat = 1000f;
			PoliticalPriority = SimRandom.RandomRange(1, 4);
			ReligiousPriority = SimRandom.RandomRange(1, 4);
			EconomicPriority = SimRandom.RandomRange(1, 4);
			MilitaryPriority = SimRandom.RandomRange(1, 4);
		}

		public Faction(int population, int influence, int wealth, int politicalPriority, int economicPriority, int religiousPriority, int militaryPriority, int startingTiles = 1) : this(startingTiles)
		{
			Population = population;
			Influence = influence;
			Wealth = wealth;
			PoliticalPriority = politicalPriority;
			EconomicPriority = economicPriority;
			ReligiousPriority = religiousPriority;
			MilitaryPriority = militaryPriority;
		}

		override public void DeployContext()
		{
			IncidentService.Instance.PerformIncidents((Faction)this);
		}
		override public void UpdateContext()
		{
			UpdateWealth();
			UpdatePopulation();
			UpdateInfluence();
			UpdatePERMS();
			UpdateNumIncidents();

			if(CheckDestroyed())
			{
				Die();
			}
		}

		override public void Die()
		{
			EventManager.Instance.RemoveEventHandler<RemoveContextEvent>(OnRemoveContextEvent);
			EventManager.Instance.Dispatch(new RemoveContextEvent(this));
		}

		public void CreateStartingCity()
		{
			var cells = SimulationUtilities.GetCitylessCellsFromList(ControlledTileIndices);
			var city = new City(this, new Location(SimRandom.RandomEntryFromList(cells)), 10, 0);
			Cities.Add(city);
			SimulationManager.Instance.world.AddContext(city);
		}

		public void CreateStartingGovernment()
		{
			Government = new Government(this);
			Government.SelectNewLeader();
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
				if(Cities.Contains((City)gameEvent.context))
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
			populationFloat *= 1.011f;
			if(MilitaryPower < (populationFloat/10))
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