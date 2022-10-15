using Game.IO;
using Game.Terrain;
using System.IO;
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
			CreateWorld();
		}

		public void CreateWorld()
		{
			mapGenerator.GenerateMap(worldChunksX * HexMetrics.chunkSizeX, worldChunksZ * HexMetrics.chunkSizeZ);
			world = new World(hexGrid);
		}

		public void SaveWorld(string mapName)
		{
			string[] directoriesAtRoot = Directory.GetDirectories(SaveUtilities.ROOT, mapName);
			if (directoriesAtRoot == null || directoriesAtRoot.Length == 0)
			{
				SaveUtilities.CreateMapDirectories(mapName);
			}
			SaveUtilities.SaveHexMapData(hexGrid, SaveUtilities.GetHexMapData(mapName));

			world.Save(mapName);
		}

		public void LoadWorld(string mapName)
		{
			SaveUtilities.LoadHexMapData(hexGrid, SaveUtilities.GetHexMapData(mapName));
			world = World.Load(hexGrid, mapName);
		}
	}
}
