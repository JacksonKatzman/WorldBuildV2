using Game.Incidents;
using Game.Terrain;
using Game.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;

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
			ContextDictionaryProvider.SetContextsProviders(() => CurrentContexts, () => AllContexts);

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

			SaveUtilities.GetOrCreateMapDirectory(mapName);
			SaveUtilities.SaveHexMapData(HexGrid, SaveUtilities.GetHexMapData(mapName));
			ES3.Save(mapName, world, SaveUtilities.GetWorldPath(mapName));
			IncidentService.Instance.SaveIncidentLog(mapName);
			AdventureService.Instance.Save(mapName);
		}

		public void LoadWorld(string mapName)
		{
			ContextDictionaryProvider.SetContextsProviders(() => CurrentContexts, () => AllContexts);

			if (string.IsNullOrEmpty(mapName))
			{
				mapName = "TEST";
			}

			SaveUtilities.LoadHexMapData(HexGrid, SaveUtilities.GetHexMapData(mapName));

			world = new World();
			world = ES3.Load<World>(mapName, SaveUtilities.GetWorldPath(mapName));
			world.LoadContextProperties();
			IncidentService.Instance.LoadIncidentLog(mapName);
			AdventureService.Instance.Load(mapName);

			OutputLogger.Log("World Loaded!");
		}

		public async void AsyncRun()
		{
			EventManager.Instance.Dispatch(new ShowLoadingScreenEvent("Generating World"));

			var startTime = Time.realtimeSinceStartup;
			await Task.Run(() => RunSimulation());
			var simTime = Time.realtimeSinceStartup - startTime;
			OutputLogger.Log("TIME TO SIM: " + simTime);

			EventManager.Instance.Dispatch(new HideLoadingScreenEvent());

			world.BeginPostGeneration();
		}

		private void RunSimulation()
		{
			for(int i = 0; i < 100; i++)
			{
				world.AdvanceTime();
			}

			//world.BeginPostGeneration();

			/*
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
			*/
		}
	}
}
