using Game.Simulation;
using Game.Terrain;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class HexCollection : InertIncidentContext, ITerrainTypeAffiliated, ILocationAffiliated
	{
		public override string Name => "Hex Collection " + ID;
		public List<int> cellCollection;
		public BiomeTerrainType AffiliatedTerrainType { get; set; }
		public bool IsMountainous { get; set; }

		public Location CurrentLocation { get; private set; }

		public HexCollection()
		{
			cellCollection = new List<int>();
		}

		public HexCollection(int firstId)
		{
			cellCollection = new List<int>();
			cellCollection.Add(firstId);
		}

		public void Initialize(HexGrid grid)
		{
			var hasMountains = 0;
			foreach(var cellIndex in cellCollection)
			{
				if(grid.GetCell(cellIndex).Elevation > 3)
				{
					hasMountains++;
				}
			}

			IsMountainous = hasMountains >= (cellCollection.Count / 2) + 1 ? true : false;
			CurrentLocation = GetCenter();
		}

		private Location GetCenter()
		{
			//temporary
			var location = new Location(cellCollection[0]);
			EventManager.Instance.Dispatch(new AddContextEvent(location, true));
			return location;
		}
	}
}