using Game.Simulation;
using Game.Terrain;

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
			return SimulationManager.Instance.HexGrid.GetCell(location.CurrentLocation.ID);
		}

		public static int GetDistanceBetweenLocations(this ILocationAffiliated from, ILocationAffiliated to)
		{
			return from.GetHexCell().coordinates.DistanceTo(to.GetHexCell().coordinates);
		}
	}
}