using Game.Incidents;
using Game.Simulation;
using Game.Terrain;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Factions
{
	[Serializable]
	public class Faction : IIncidentContext
	{
		public Type ContextType => typeof(Faction);

		public int NumIncidents { get; set; }

		public int ParentID => -1;
		public int Population { get; set; }
		public int Influence { get; set; }
		public int Wealth { get; set; }
		public Dictionary<IIncidentContext, int> FactionRelations { get; set; }
		public int ControlledTiles => controlledTileIndices.Count;

		[HideInInspector]
		public List<int> controlledTileIndices;

		public Faction()
		{
		}

		public Faction(int startingTiles)
		{
			AttemptExpandBorder(startingTiles);
		}

		public void DeployContext()
		{
			IncidentService.Instance.PerformIncidents(this);
		}
		public void UpdateContext()
		{
			NumIncidents = 1;
			Influence += 1;
		}

		public bool AttemptExpandBorder(int numTimes)
		{
			HexCellPriorityQueue searchFrontier = new HexCellPriorityQueue();
			int searchFrontierPhase = 1;
			int size = 0;

			if (controlledTileIndices == null)
			{
				controlledTileIndices = new List<int>();
			}
			if(controlledTileIndices.Count == 0)
			{
				if (SimulationUtilities.GetRandomUnclaimedCellIndex(out var index))
				{
					controlledTileIndices.Add(index);
					size++;
				}
				else
				{
					OutputLogger.LogError("Couldn't find free tile to create faction on!");
					return false;
				}
			}

			HexCell firstCell = SimulationManager.Instance.HexGrid.GetCell(controlledTileIndices[0]);
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
					controlledTileIndices.Add(current.Index);
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
	}
}