using Game.Terrain;
using System.Collections.Generic;
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

		[SerializeField]
		public List<FactionPreset> factions;

		private void Awake()
		{
			var simMan = SimulationManager.Instance;
			simMan.HexGrid = hexGrid;
			hexGrid.Initalize();
			simMan.MapGenerator = mapGenerator;
			simMan.WorldChunksX = worldChunksX;
			simMan.WorldChunksZ = worldChunksZ;

			simMan.CreateWorld(factions);
		}
	}
}
