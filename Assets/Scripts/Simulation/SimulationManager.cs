using Game.Incidents;
using Game.Terrain;
using Game.Utilities;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

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


		public IncidentContextDictionary Contexts => world.AllContexts;

		public void CreateWorld(List<FactionPreset> factions)
		{
			MapGenerator.GenerateMap(WorldChunksX * HexMetrics.chunkSizeX, WorldChunksZ * HexMetrics.chunkSizeZ);
			world = new World(HexGrid);
			world.Initialize(factions);
			var test = AdventureService.Instance;
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
			var table = world.GetDataTable();

			foreach(var contextList in world.AllContexts.Values)
			{
				foreach (var context in contextList)
				{
					var contextTable = context.GetDataTable();
					table.Merge(contextTable);
				}
			}

			table.ToCSV(Application.dataPath + "/Resources/" + "factionCSV" + ".csv");
			IncidentService.Instance.WriteIncidentLogToDisk();
		}
	}
}
