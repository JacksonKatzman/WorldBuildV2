using Game.Simulation;
using Game.Terrain;
using Newtonsoft.Json;
using System;

namespace Game.Incidents
{
	public class Location : InertIncidentContext, ILocationAffiliated, ITerrainTypeAffiliated, IEquatable<Location>
	{
		public override Type ContextType => typeof(Location);
		public int TileIndex { get; set; }

		public Location CurrentLocation => this;
		[JsonIgnore]
		public BiomeTerrainType AffiliatedTerrainType => SimulationManager.Instance.HexGrid.cells[TileIndex].BiomeSubtype;

        public override string Description => $"LOCATION DESCRIPTION";

        public Location() { }
		public Location(int tileIndex)
		{
			TileIndex = tileIndex;
		}

		public bool Equals(Location other)
		{
			return TileIndex == other.TileIndex;
		}
	}
}