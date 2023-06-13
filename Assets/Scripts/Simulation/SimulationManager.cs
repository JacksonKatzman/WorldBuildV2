using Game.Incidents;
using Game.Terrain;
using Game.Utilities;
using System;
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


		public IncidentContextDictionary CurrentContexts => world.CurrentContexts;
		public IncidentContextDictionary AllContexts => world.AllContexts;

		public void CreateWorld(List<FactionPreset> factions)
		{
			MapGenerator.GenerateMap(WorldChunksX * HexMetrics.chunkSizeX, WorldChunksZ * HexMetrics.chunkSizeZ);
			world = new World();
			world.Initialize(factions);
			var test = AdventureService.Instance;
		}

		public void CreateDebugWorld()
		{
			world = new World();
		}

		public void SaveWorld(string mapName)
		{
			if(string.IsNullOrEmpty(mapName))
			{
				mapName = "TEST";
			}
			string[] directoriesAtRoot = Directory.GetDirectories(SaveUtilities.ROOT, mapName);
			if (directoriesAtRoot == null || directoriesAtRoot.Length == 0)
			{
				SaveUtilities.CreateMapDirectories(mapName);
			}
			SaveUtilities.SaveHexMapData(HexGrid, SaveUtilities.GetHexMapData(mapName));

			ES3.Save(mapName, world);
		
		}

		public void LoadWorld(string mapName)
		{
			if (string.IsNullOrEmpty(mapName))
			{
				mapName = "TEST";
			}
			SaveUtilities.LoadHexMapData(HexGrid, SaveUtilities.GetHexMapData(mapName));

			var loadedWorld = ES3.Load<World>(mapName, world);
			loadedWorld.LoadContextProperties();
			world = loadedWorld;

			OutputLogger.Log("World Loaded!");
		}

		public void DebugRun()
		{
			for(int i = 0; i < 100; i++)
			{
				world.AdvanceTime();
			}

			world.BeginPostGeneration();

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
