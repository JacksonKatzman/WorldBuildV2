using System;

namespace Game.Incidents
{
	public class Location : InertIncidentContext, ILocationAffiliated
	{
		public override Type ContextType => typeof(Location);
		public int TileIndex { get; set; }

		public Location CurrentLocation => this;

		public Location() { }
		public Location(int tileIndex)
		{
			TileIndex = tileIndex;
		}
	}
}