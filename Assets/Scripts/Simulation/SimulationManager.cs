using Game.Incidents;
using Game.IO;
using Game.Terrain;
using System.IO;

namespace Game.Simulation
{
	public class SimulationManager
	{
		public HexGrid HexGrid { get; set; }

		public HexMapGenerator MapGenerator { get; set; }

		public int WorldChunksX { get; set; }
		public int WorldChunksZ { get; set; }

		public World world;

		private static SimulationManager instance;
		public static SimulationManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new SimulationManager();
				}
				return instance;
			}
		}

		private SimulationManager()
		{
			OutputLogger.Log("Sim Manager Made!");
		}


		public ContextTypeListDictionary<IIncidentContext> Contexts => world.Contexts;

		public void CreateWorld()
		{
			MapGenerator.GenerateMap(WorldChunksX * HexMetrics.chunkSizeX, WorldChunksZ * HexMetrics.chunkSizeZ);
			world = new World(HexGrid);
			world.Initialize();
		}

		public void CreateDebugWorld()
		{
			world = new World();
		}

		public void SaveWorld(string mapName)
		{
			string[] directoriesAtRoot = Directory.GetDirectories(SaveUtilities.ROOT, mapName);
			if (directoriesAtRoot == null || directoriesAtRoot.Length == 0)
			{
				SaveUtilities.CreateMapDirectories(mapName);
			}
			SaveUtilities.SaveHexMapData(HexGrid, SaveUtilities.GetHexMapData(mapName));

			world.Save(mapName);
		}

		public void LoadWorld(string mapName)
		{
			SaveUtilities.LoadHexMapData(HexGrid, SaveUtilities.GetHexMapData(mapName));
			world = World.Load(HexGrid, mapName);
		}

		public void DebugRun()
		{
			for(int i = 0; i < 100; i++)
			{
				world.AdvanceTime();
			}
		}
	}
}
