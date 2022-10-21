using Game.Terrain;
using UnityEngine;

namespace Game.Simulation
{
	public class SimulationManagerSetup : MonoBehaviour
	{
		[SerializeField]
		public HexGrid hexGrid;

		[SerializeField]
		public HexMapGenerator mapGenerator;

		[SerializeField]
		private int worldChunksX = 16, worldChunksZ = 12;

		private void Awake()
		{
			var simMan = SimulationManager.Instance;
			simMan.HexGrid = hexGrid;
			simMan.MapGenerator = mapGenerator;
			simMan.WorldChunksX = worldChunksX;
			simMan.WorldChunksZ = worldChunksZ;

			simMan.CreateWorld();
		}
	}
}
