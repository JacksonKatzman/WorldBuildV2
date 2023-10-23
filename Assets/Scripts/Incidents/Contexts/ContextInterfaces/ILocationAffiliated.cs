using Game.Simulation;
using Game.Terrain;
using System.Collections.Generic;

namespace Game.Incidents
{
	public interface ILocationAffiliated
	{
		Location CurrentLocation { get; }
	}

	public static class LocationAffiliatedExtensions
	{
		public static HexCell GetHexCell(this ILocationAffiliated location)
		{
			return SimulationManager.Instance.HexGrid.GetCell(location.CurrentLocation.TileIndex);
		}

		public static int GetDistanceBetweenLocations(this ILocationAffiliated from, ILocationAffiliated to)
		{
			return from.GetHexCell().coordinates.DistanceTo(to.GetHexCell().coordinates);
		}

		public static List<HexCell> GetAllCellsInRange(this ILocationAffiliated location, int range)
		{
			return SimulationUtilities.GetAllCellsInRange(GetHexCell(location), range);
		}
	}
}