using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.WorldGeneration;
using Game.Generators.Noise;
using Game.Enums;

public class WorldHandler : MonoBehaviour
{
    private static WorldHandler instance;
    public static WorldHandler Instance => instance;

    public NoiseSettings settings;
    public BiomeContainer biomeSettings;

    [SerializeField]
    private Renderer coloredMapRenderer;

    [SerializeField]
    private Renderer heightMapRenderer;

    [SerializeField]
    private Renderer rainfallMapRenderer;

    [SerializeField]
    private Renderer fertilityMapRenderer;

    public int seed;
    System.Random seededRandom;

    World world;
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

        for (int a = 0; a < 1; a++)
        {
            world.SpawnRandomCity();
            //string fullName = NameGenerator.GeneratePersonFullName(nameContainers[0], Gender.NEITHER);
            //Debug.Log(fullName);
        }
    }

    public int RandomInteger()
	{
        return seededRandom.Next(-100000, 100000);
    }

    public float RandomFloat01()
	{
        float percision = 100000.0f;
        return seededRandom.Next(0, (int)percision) / percision;
    }

    public int RandomRange(int minValue, int maxValue)
	{
        return seededRandom.Next(minValue, maxValue);

    }

    public void DrawNoiseMap()
	{
        Vector2Int noiseSize = new Vector2Int(settings.worldSize.x * chunkSize, settings.worldSize.y * chunkSize);
        world = WorldGenerator.GenerateWorld(settings, chunkSize, biomeSettings.biomes);

        heightMapRenderer.sharedMaterial.mainTexture = world.heightMapTexture;
        coloredMapRenderer.transform.localScale = new Vector3(world.biomeMap.GetLength(0), 1, world.biomeMap.GetLength(1));

        rainfallMapRenderer.sharedMaterial.mainTexture = DebugCreateNoiseTexture(world.noiseMaps[Game.Enums.MapCategory.RAINFALL]);
        coloredMapRenderer.transform.localScale = new Vector3(world.biomeMap.GetLength(0), 1, world.biomeMap.GetLength(1));

        fertilityMapRenderer.sharedMaterial.mainTexture = DebugCreateNoiseTexture(world.noiseMaps[Game.Enums.MapCategory.FERTILITY]);
        coloredMapRenderer.transform.localScale = new Vector3(world.biomeMap.GetLength(0), 1, world.biomeMap.GetLength(1));

        coloredMapRenderer.sharedMaterial.mainTexture = world.colorMapTexture;
        coloredMapRenderer.transform.localScale = new Vector3(world.biomeMap.GetLength(0), 1, world.biomeMap.GetLength(1));
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
