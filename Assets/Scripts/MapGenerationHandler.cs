using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapMagic.Core;
using System.Linq;
using Pinwheel.Vista;
using Pinwheel.Vista.Graph;

public class MapGenerationHandler : MonoBehaviour
{
    //public MapMagicObject mmObj;
    //public Texture2D noiseTexture;
    public VistaManager vMan;
    public BiomeContainer biomes;
    public AnimationCurve temperatureCurve;

    public Vector2Int noiseVector;
    public float noiseScale;
    public int octaves;
    public float persistence;
    public float lacunarity;
    public Vector2 offset;

    public Renderer mapRenderer;

    void Start()
    {
        //GenerateMapTexture();
    }

    public void DrawMap()
	{
        var heightMap = NoiseGenerator.GeneratePerlinNoise(noiseVector, noiseScale,
                                                         octaves, persistence,
                                                          lacunarity, offset);

        var width = heightMap.GetLength(0);
        var height = heightMap.GetLength(1);

        Texture2D tex = new Texture2D(width, height);
        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }
        tex.SetPixels(colorMap);
        tex.Apply();

        mapRenderer.sharedMaterial.mainTexture = tex;
        mapRenderer.transform.localScale = new Vector3(width, 1, height);
    }

    void GenerateMapTexture()
	{
        var desiredSize = 3;//noiseVector.x;
        var tileSize = 500;
        var heightMap = NoiseGenerator.GeneratePerlinNoise(noiseVector, noiseScale,
                                                         octaves, persistence,
                                                          lacunarity, offset);
        var rainfallMap = NoiseGenerator.GeneratePerlinNoise(noiseVector, noiseScale,
                                                         octaves, persistence,
                                                          lacunarity, offset);
        var fertilityMap = NoiseGenerator.GeneratePerlinNoise(noiseVector, noiseScale,
                                                         octaves, persistence,
                                                          lacunarity, offset);

        LocalProceduralBiome[,] biomesGrid = new LocalProceduralBiome[desiredSize, desiredSize];

        for(int x = 0; x < desiredSize; x++)
		{
            for(int z = 0; z < desiredSize; z++)
			{
                LocalProceduralBiome biome = LocalProceduralBiome.CreateInstanceInScene(vMan);
                biome.dataMask = (BiomeDataMask)CombineAllBiomeMasks();
                biome.baseResolution = 1024;
                biome.falloffDistance = tileSize * 0.5f;
                biome.biomeMaskResolution = 512;
                biome.anchors = new Vector3[]
                {
                    new Vector3(tileSize*x, 0, tileSize*z), new Vector3(tileSize*x, 0, tileSize*(z+1)), new Vector3(tileSize*(x+1), 0, tileSize*(z+1)), new Vector3(tileSize*(x+1), 0, tileSize*z)
                };

                var temperature = temperatureCurve.Evaluate(x / (float)desiredSize);
                biome.terrainGraph = biomes.DetermineBiome(heightMap[x, z], rainfallMap[x, z], fertilityMap[x, z], temperature).biomeGraph;
                //biome.terrainGraph = biomes.biomes[0].biomeGraph;
                biomesGrid[x, z] = biome;
                if (biome.terrainGraph != null)
                {
                    biome.CleanUp();
                    biome.MarkChanged();
                }
            }
		}

        vMan.Generate();
	}
    /*
    float[,] GenerateTemperatureGradient(int noiseSize)
	{
        float[,] gradient = new float[noiseSize, noiseSize];
        var half = (noiseSize / 2);
        var val = 

        for (int x = 0; x < noiseSize; x++)
        {
            for (int z = 0; z < noiseSize; z++)
            {

            }
        }
    }
    */

    int CombineAllBiomeMasks()
	{
        var combined = 1 + 2 + 4 + 8 + 16 + 32 + 64 + 128 + 256 + 512 + 1024 + 2048;
        return combined;
            /*
        HeightMap = 1,
        HoleMap = 2,
        MeshDensityMap = 4,
        AlbedoMap = 8,
        MetallicMap = 16,
        LayerWeightMaps = 32,
        TreeInstances = 64,
        DetailDensityMaps = 128,
        DetailInstances = 256,
        ObjectInstances = 512,
        GenericTextures = 1024,
        GenericBuffers = 2048
            */
    }
}
