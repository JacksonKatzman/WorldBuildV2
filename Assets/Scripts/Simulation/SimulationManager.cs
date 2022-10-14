using Game.Terrain;
using UnityEngine;

namespace Game.Simulation
{
	public class SimulationManager : MonoBehaviour
	{
		[SerializeField]
		public HexGrid hexGrid;

		[SerializeField]
		public HexMapGenerator mapGenerator;

		[SerializeField]
		private int worldChunksX = 16, worldChunksZ = 12;

		public World world;

		private void Awake()
		{
			hexGrid.Initalize();
			mapGenerator.GenerateMap(worldChunksX * HexMetrics.chunkSizeX, worldChunksZ * HexMetrics.chunkSizeZ);
			world = new World(hexGrid);
		}
	}
}
