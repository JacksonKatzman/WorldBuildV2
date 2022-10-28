using Game.Incidents;
using Game.Simulation;
using Game.Terrain;
using System;
using System.Collections.Generic;

namespace Game.Factions
{
	[Serializable]
	public class Faction : IIncidentContext
	{
		public FactionContext context;
		public Type ContextType => typeof(FactionContext);

		public int NumIncidents => throw new NotImplementedException();

		public int ParentID => throw new NotImplementedException();

		public Faction()
		{
			context = new FactionContext();
			context.Provider = this;
		}

		public Faction(int startingTiles)
		{
			context = new FactionContext();
			context.Provider = this;
			AttemptExpandBorder(startingTiles);
		}

		public void DeployContext()
		{
			IncidentService.Instance.PerformIncidents(this);
		}

		public IIncidentContext GetContext()
		{
			return context;
		}

		public void UpdateContext()
		{
			context.Influence += 1;
			if(context.TestInts == null)
			{
				context.TestInts = new Dictionary<IIncidentContext, int>();
				context.TestInts.Add(this.context, 5);
			}
		}

		public bool AttemptExpandBorder(int numTimes)
		{
			HexCellPriorityQueue searchFrontier = new HexCellPriorityQueue();
			int searchFrontierPhase = 1;
			int size = 0;

			if (context.controlledTileIndices == null)
			{
				context.controlledTileIndices = new List<int>();
			}
			if(context.controlledTileIndices.Count == 0)
			{
				if (SimulationUtilities.GetRandomUnclaimedCellIndex(out var index))
				{
					context.controlledTileIndices.Add(index);
					size++;
				}
				else
				{
					OutputLogger.LogError("Couldn't find free tile to create faction on!");
					return false;
				}
			}

			HexCell firstCell = SimulationManager.Instance.HexGrid.GetCell(context.controlledTileIndices[0]);
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
					context.controlledTileIndices.Add(current.Index);
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