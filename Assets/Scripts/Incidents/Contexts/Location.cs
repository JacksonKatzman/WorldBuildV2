using Game.Simulation;
using Game.Terrain;
using Newtonsoft.Json;
using System;

namespace Game.Incidents
{
	public class Location : InertIncidentContext, ILocationAffiliated, ITerrainTypeAffiliated
	{
		public override Type ContextType => typeof(Location);
		public int TileIndex { get; set; }

		public Location CurrentLocation => this;
		[JsonIgnore]
		public BiomeTerrainType AffiliatedTerrainType => SimulationManager.Instance.HexGrid.cells[TileIndex].TerrainType;

		public Location() { }
		public Location(int tileIndex)
		{
			TileIndex = tileIndex;
		}
	}
}