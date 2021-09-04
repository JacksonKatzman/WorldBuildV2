using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.WorldGeneration;
using Game.Generators.Noise;
using Game.Enums;
using Game.Factions;
using Game.Generators;
using Game.Debug;
using Game.Data.EventHandling.EventRecording;
using Game.Visuals;

public class SimulationManager : MonoBehaviour
{
    private static SimulationManager instance;
    public static SimulationManager Instance => instance;
    public World World => world;

    public NoiseSettings settings;
    public BiomeContainer biomeSettings;

    [SerializeField]
    MeshFilter mapMeshFilter;

    [SerializeField]
    MeshRenderer mapMeshRenderer;

    [SerializeField]
    MeshFilter factionMeshFilter;

    [SerializeField]
    MeshRenderer factionMeshRenderer;

    [SerializeField]
    GameObject cityMarker;

    [SerializeField]
    WorldVisualBuilder visualBuilder;

    private List<GameObject> cityMarkers;

    public EventRecorder eventRecorder;

    public float heightMulitplier;
    public AnimationCurve heightCurve;

    [SerializeField]
    private Renderer coloredMapRenderer;

    public int seed;
    public System.Random seededRandom;

    private World world;
    int chunkSize = 10;

    public bool DebugPause = false;
    private int debugYearsPassed = 0;
    public TimingProfiler timer;

	private void Awake()
	{
        if(instance != null && instance != this)
		{
            Destroy(this.gameObject);
		}
        else
		{
            instance = this;
		}
	}

	void Start()
    {
        timer = new TimingProfiler();

        cityMarkers = new List<GameObject>();

        eventRecorder = new EventRecorder();

        seededRandom = new System.Random(seed);

        GenerateWorld();

        //DrawNoiseMap();

        //DrawMesh(MeshGenerator.GenerateVoxelTerrainMesh(world.noiseMaps[Game.Enums.MapCategory.TERRAIN], heightMulitplier, heightCurve), world.voxelColorMapTexture);

        world.HandleDeferredActions();

        //RedrawFactionMap();
    }

    public void GenerateWorld()
	{
        world = WorldGenerator.GenerateWorld(settings, chunkSize, biomeSettings.biomes);

        for (int a = 0; a < 10; a++)
        {
            FactionGenerator.SpawnFaction(world);
        }

        GenerateWorldVisuals();
    }

    private void GenerateWorldVisuals()
	{
        visualBuilder.BuildWorld(world);
        visualBuilder.UpdateFactionBorders();
	}

    public void DrawNoiseMap()
	{
        Vector2Int noiseSize = new Vector2Int(settings.worldSize.x * chunkSize, settings.worldSize.y * chunkSize);
        world = WorldGenerator.GenerateWorld(settings, chunkSize, biomeSettings.biomes);
        /*
        heightMapRenderer.sharedMaterial.mainTexture = world.heightMapTexture;
        coloredMapRenderer.transform.localScale = new Vector3(world.biomeMap.GetLength(0), 1, world.biomeMap.GetLength(1));

        rainfallMapRenderer.sharedMaterial.mainTexture = DebugCreateNoiseTexture(world.noiseMaps[Game.Enums.MapCategory.RAINFALL]);
        coloredMapRenderer.transform.localScale = new Vector3(world.biomeMap.GetLength(0), 1, world.biomeMap.GetLength(1));

        fertilityMapRenderer.sharedMaterial.mainTexture = DebugCreateNoiseTexture(world.noiseMaps[Game.Enums.MapCategory.FERTILITY]);
        coloredMapRenderer.transform.localScale = new Vector3(world.biomeMap.GetLength(0), 1, world.biomeMap.GetLength(1));
        */
        coloredMapRenderer.sharedMaterial.mainTexture = world.colorMapTexture;
        coloredMapRenderer.transform.localScale = new Vector3(world.biomeMap.GetLength(0) * -1, 1, world.biomeMap.GetLength(1));
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
	{
        mapMeshFilter.sharedMesh = meshData.CreateMesh();
        mapMeshRenderer.sharedMaterial.mainTexture = texture;
	}

    public void DrawMesh(VoxelMeshData meshData, Texture2D texture)
    {
        var mesh = meshData.CreateMesh();
        mapMeshFilter.sharedMesh = mesh;
        mapMeshRenderer.sharedMaterial.mainTexture = texture;

        factionMeshFilter.sharedMesh = mesh;
        factionMeshRenderer.sharedMaterial.mainTexture = world.CreateFactionMap();
    }

    private void RedrawFactionMap()
	{
        factionMeshRenderer.sharedMaterial.mainTexture = world.CreateFactionMap();
        
        for(int i = 0; i < cityMarkers.Count; i++)
		{
            Destroy(cityMarkers[i]);
		}
        cityMarkers.Clear();

        foreach(Faction faction in world.factions)
		{
            foreach(City city in faction.cities)
			{
                var marker = Instantiate(cityMarker);
                var terrainMap = world.noiseMaps[MapCategory.TERRAIN];
                var width = terrainMap.GetLength(0);
                var height = terrainMap.GetLength(1);
                var pos = city.tile.GetWorldPosition();
                var markerPos = new Vector2((width / -2) + pos.x + 0.5f, (height/2) - pos.y - 0.5f) * 10;

                var heightMapHeight = terrainMap[pos.x, pos.y];
                heightMapHeight *= heightMulitplier;
                marker.transform.localPosition = new Vector3(markerPos.x, 1, markerPos.y);
                marker.transform.parent = factionMeshFilter.transform;
                cityMarkers.Add(marker);
            }
		}
        
    }

    public void DebugAdvanceTime()
	{
        //world.AdvanceTime();
        DebugRunSimulation();
	}

    private void DebugRunSimulation()
	{
        DebugPause = false;
        debugYearsPassed = 0;
        var startTime = Time.realtimeSinceStartup;
        while(DebugPause == false && debugYearsPassed < 200)
		{
            //Debug.Log("ADVANCING TIME!");
            world.AdvanceTime();
            debugYearsPassed++;
		}
        timer.PrintFindings();
        OutputLogger.LogFormat("Full generation took {0} seconds.", LogSource.PROFILE, Time.realtimeSinceStartup - startTime);
        //world.HandleCleanup();
        //RedrawFactionMap();
        visualBuilder.UpdateFactionBorders();
        OutputLogger.LogFormat("Years passed since world generation: {0}", LogSource.MAIN, world.yearsPassed);
	}

    private Texture2D DebugCreateNoiseTexture(float[,] noiseMap)
	{
        var width = noiseMap.GetLength(0);
        var height = noiseMap.GetLength(1);

        Texture2D worldNoiseTexture = new Texture2D(width, height);
        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }

        worldNoiseTexture.SetPixels(colorMap);
        worldNoiseTexture.Apply();

        return worldNoiseTexture;
    }
}
