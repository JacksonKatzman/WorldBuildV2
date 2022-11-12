using Game.Incidents;
using Game.Simulation;
using Game.Terrain;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{
	[Serializable]
	public class Faction : IIncidentContext, IFactionAffiliated
	{
		public Type ContextType => typeof(Faction);

		public int NumIncidents { get; set; }

		public int ParentID => -1;
		public Faction AffiliatedFaction => this;
		public int Population { get; set; }
		public int Influence { get; set; }
		public int Wealth { get; set; }
		public Dictionary<IIncidentContext, int> FactionRelations { get; set; }
		public int ControlledTiles => ControlledTileIndices.Count;
		public List<City> Cities { get; set; }
		public int PoliticalPriority { get; set; }
		public int EconomicPriority { get; set; }
		public int ReligiousPriority { get; set; }
		public int MilitaryPriority { get; set; }
		public List<Faction> FactionsAtWarWith { get; set; }

		public bool AtWar => FactionsAtWarWith.Count > 0;
		virtual public bool CanExpandTerritory => true;
		virtual public bool CanTakeMilitaryAction => true;
		public Government Government { get; set; }

		[HideInInspector]
		public List<int> ControlledTileIndices { get; set; }

		public Faction()
		{
			Cities = new List<City>();
			FactionsAtWarWith = new List<Faction>();
			CreateStartingCity();
			CreateStartingGovernment();
		}

		public Faction(int startingTiles)
		{
			AttemptExpandBorder(startingTiles);
		}

		public Faction(int population, int influence, int wealth, int politicalPriority, int economicPriority, int religiousPriority, int militaryPriority) : this()
		{
			Population = population;
			Influence = influence;
			Wealth = wealth;
			PoliticalPriority = politicalPriority;
			EconomicPriority = economicPriority;
			ReligiousPriority = religiousPriority;
			MilitaryPriority = militaryPriority;
		}

		public void DeployContext()
		{
			IncidentService.Instance.PerformIncidents((Faction)this);
		}
		public void UpdateContext()
		{
			UpdateWealth();
			UpdatePopulation();
			UpdateInfluence();
			UpdatePERMS();
			UpdateNumIncidents();
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
			if(ControlledTileIndices.Count == 0)
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
				if(SimulationUtilities.IsCellIndexUnclaimed(current.Index))
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

		private void UpdateInfluence()
		{

		}

		private void UpdateWealth()
		{
			foreach(var city in Cities)
			{
				Wealth += city.GenerateWealth();
			}
		}

		private void UpdatePopulation()
		{

		}

		private void UpdatePERMS()
		{

		}

		private void UpdateNumIncidents()
		{
			NumIncidents = 1;
		}
	}
}