using Game.Incidents;
using Game.Terrain;
using Game.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Game.Data;
using Game.Debug;

namespace Game.Simulation
{
	public class SimulationManager
	{
		public HexGrid HexGrid { get; set; }

		public HexMapGenerator MapGenerator { get; set; }

		public int WorldChunksX { get; set; }
		public int WorldChunksZ { get; set; }

		public World world;

		public SimulationOptions simulationOptions;

		public CancellationTokenSource cancellationTokenSource;

		private StatTracker statTracker;

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
			cancellationTokenSource = new CancellationTokenSource();
			statTracker = new StatTracker();
			OutputLogger.Log("Sim Manager Made!");
		}


		public IncidentContextDictionary CurrentContexts => world.CurrentContexts;
		public IncidentContextDictionary AllContexts => world.AllContexts;

		public void CreateWorld(SimulationOptions options)
		{
			simulationOptions = options;

			IncidentService.Instance.Setup();
			UserInterfaceService.Instance.incidentWiki.Clear();
			ContextDictionaryProvider.SetContextsProviders(() => CurrentContexts, () => AllContexts);
			ContextDictionaryProvider.AllowImmediateChanges = true;

			world = new World();
			MapGenerator.GenerateMap(WorldChunksX * HexMetrics.chunkSizeX, WorldChunksZ * HexMetrics.chunkSizeZ);
			world.Initialize(options);
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
			ContextDictionaryProvider.AllowImmediateChanges = true;

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

		public async UniTask AsyncRun()
		{
			EventManager.Instance.Dispatch(new ShowLoadingScreenEvent("Generating World"));

			var startTime = Time.realtimeSinceStartup;
			GameProfiler.UpdateProfiler = true;

			await RunSimulationWithCancellation(cancellationTokenSource.Token);
			await CompileWikiWithCancellation(cancellationTokenSource.Token);
			var simTime = Time.realtimeSinceStartup - startTime;
			OutputLogger.Log("TIME TO SIM: " + simTime);
			GameProfiler.UpdateProfiler = false;

			EventManager.Instance.Dispatch(new HideLoadingScreenEvent());
			statTracker.ReportDeathAges();

			UniTask.ReturnToMainThread();
			world.PostSimulationCleanup();
			world.BeginPostGeneration();
		}

		public async UniTask RunSimulationWithCancellation(CancellationToken token)
		{
			int yearsPassed = 0;
			while(!token.IsCancellationRequested && yearsPassed < world.simulationOptions.simulatedYears)
			{
				await world.AdvanceTime();
				yearsPassed++;
			}
		}

		public async UniTask CompileWikiWithCancellation(CancellationToken token)
		{
			while (!token.IsCancellationRequested && !UserInterfaceService.Instance.incidentWiki.initialized)
			{
				await UserInterfaceService.Instance.incidentWiki.InitializeWiki();
			}
		}

		private void RunSimulation()
		{
			/*
			for(int i = 0; i < world.simulationOptions.simulatedYears; i++)
			{
				world.AdvanceTime();
			}
			*/

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
