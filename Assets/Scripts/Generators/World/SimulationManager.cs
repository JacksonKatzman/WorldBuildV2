using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.WorldGeneration;
using Game.Generators.Noise;
using Game.Enums;
using Game.Generators;

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
        seededRandom = new System.Random(seed);

        DrawNoiseMap();

        DrawMesh(MeshGenerator.GenerateVoxelTerrainMesh(world.noiseMaps[Game.Enums.MapCategory.TERRAIN], heightMulitplier, heightCurve), world.voxelColorMapTexture);

        for (int a = 0; a < 10; a++)
        {
            FactionGenerator.SpawnFaction(world);
        }
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
        coloredMapRenderer.sharedMaterial.mainTexture = world.voxelColorMapTexture;
        coloredMapRenderer.transform.localScale = new Vector3(world.biomeMap.GetLength(0)*2, 1, world.biomeMap.GetLength(1)*2);
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
        while(DebugPause == false && debugYearsPassed < 100)
		{
            world.AdvanceTime();
            debugYearsPassed++;
		}
        RedrawFactionMap();
        OutputLogger.LogFormat("Years passed since world generation: {0}", LogSource.WORLDGEN, world.yearsPassed);
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
