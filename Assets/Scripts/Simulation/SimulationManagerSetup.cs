using Game.Incidents;
using Game.Terrain;
using Sirenix.OdinInspector;
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
		public SimulationOptions options;

		[SerializeField]
		public FlavorService flavorService;

		[SerializeField]
		private int worldChunksX = 16, worldChunksZ = 12;

		[Button("Create New World")]
		public void CreateNewWorld()
		{
			var simMan = SimulationManager.Instance;
			simMan.MapGenerator = mapGenerator;
			simMan.WorldChunksX = worldChunksX;
			simMan.WorldChunksZ = worldChunksZ;

			SimulationManager.Instance.CreateWorld(options);
		}

		private void Awake()
		{
			var simMan = SimulationManager.Instance;
			simMan.HexGrid = hexGrid;
			hexGrid.Initalize();
			flavorService.Init();
		}

		private void Start()
		{
			IncidentService.Instance.CompileIncidents();

			CreateNewWorld();

			HexMapCamera.CenterPosition();
		}

		public void TestRun()
		{
			SimulationManager.Instance.AsyncRun();
		}

		private void OnDestroy()
		{
			SimulationManager.Instance.cancellationTokenSource.Cancel();
		}
	}
}
