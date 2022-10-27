using Game.Factions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{
	[Serializable]
	public class FactionContext : IIncidentContext
	{
		public IIncidentContextProvider Provider { get; set; }
		public Type ContextType => typeof(FactionContext);
		public int NumIncidents { get; set; }
		public int ParentID => -1;
		public int Population { get; set; }
		public int Influence { get; set; }
		public Dictionary<IIncidentContext, int> TestInts { get; set; }
		public int ControlledTiles => controlledTileIndices.Count;

		[HideInInspector]
		public List<int> controlledTileIndices;
	}
}