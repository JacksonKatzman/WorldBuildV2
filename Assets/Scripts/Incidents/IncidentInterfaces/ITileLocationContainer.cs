using Game.WorldGeneration;
using System.Collections.Generic;

namespace Game.Incidents
{
	public interface ITileLocationContainer
	{
		public List<Tile> Locations
		{
			get;
		}
	}
}