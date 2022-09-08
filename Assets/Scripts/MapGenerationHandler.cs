using System.Collections;
using UnityEngine;
using MapMagic.Core;
using System.Linq;
using Pinwheel.Vista;
using Pinwheel.Vista.Graph;
using Game.WorldGeneration;
using System.Collections.Generic;
using Game.Math;
using Edge = System.Tuple<int, int>;
using System;
using ConcaveHull;

public class MapGenerationHandler : MonoBehaviour
{
    private static Dictionary<int, Vector2> WalkDirections = new Dictionary<int, Vector2>
    {
        {0, Vector2.up },
        {1, Vector2.right },
        {2, Vector2.down},
        {3, Vector2.left }
    };

    public VistaManager vMan;
    public BiomeContainer biomes;
    public AnimationCurve temperatureCurve;

    public Vector2Int noiseVector;
    public float noiseScale;
    public int octaves;
    public float persistence;
    public float lacunarity;
    public Vector2 offset;

    public int tileSize;

    public Renderer mapRenderer;

    void Start()
    {
        GenerateByCluster();
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

    private void GenerateByCluster()
	{
        var desiredSize = 5;//noiseVector.x;
        var heightMap = NoiseGenerator.GeneratePerlinNoise(noiseVector, noiseScale,
                                                         octaves, persistence,
                                                          lacunarity, offset);
        var rainfallMap = NoiseGenerator.GeneratePerlinNoise(noiseVector, noiseScale,
                                                         octaves, persistence,
                                                          lacunarity, offset);
        var fertilityMap = NoiseGenerator.GeneratePerlinNoise(noiseVector, noiseScale,
                                                         octaves, persistence,
                                                          lacunarity, offset);

        Biome[,] fullBiomesGrid = new Biome[desiredSize, desiredSize];
        for (int x = 0; x < desiredSize; x++)
        {
            for (int z = 0; z < desiredSize; z++)
            {
                var temperature = temperatureCurve.Evaluate(x / (float)desiredSize);
                fullBiomesGrid[x, z] = biomes.DetermineBiome(heightMap[x, z], rainfallMap[x, z], fertilityMap[x, z], temperature);
            }
        }
        var clusters = ClusterByBiome(fullBiomesGrid);
        GenerateBiomeClusters(clusters);
    }

    private List<BiomeClusterNode> ClusterByBiome(Biome[,] biomes)
    {
        var width = biomes.GetLength(0);
        var height = biomes.GetLength(1);

        var clusterList = new List<BiomeClusterNode>();
        var neighborList = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                var biome = biomes[x, z];
                if (x - 1 >= 0)
                {
                    neighborList.Add(new Vector2Int(x - 1, z));
                }
                if (z - 1 >= 0)
                {
                    neighborList.Add(new Vector2Int(x, z - 1));
                }
                if (x + 1 < width)
                {
                    neighborList.Add(new Vector2Int(x + 1, z));
                }
                if (z + 1 < height)
                {
                    neighborList.Add(new Vector2Int(x, z + 1));
                }

                var possibleClusters = clusterList.Where(x => x.biome == biome);

                var found = false;

                foreach (var cluster in possibleClusters)
                {
                    var intersection = cluster.tiles.Intersect(neighborList);
                    if (intersection.ToList().Count > 0)
                    {
                        cluster.tiles.Add(new Vector2Int(x, z));
                        found = true;
                    }
                }

                if (!found)
                {
                    clusterList.Add(new BiomeClusterNode(biome, new Vector2Int(x, z)));
                }

                neighborList.Clear();
            }
        }

        return clusterList;
    }

    private int GetNextDirection(int current)
	{
        int remainder = (current + 1) % 3;
        return (remainder < 0) ? (3 + remainder) : remainder;
	}

    private List<Vector3> WalkBounds(List<Vector2> points)
	{
        var finalPoints = new List<Vector2>();
        finalPoints.Add(points[0]);

        var current = points[0];
        var direction = 0;
        var done = false;

        while(!done)
		{
            var adjacentPoints = points.Where(x => Vector2.Distance(current, x) == tileSize);
            Vector2 pointDefault = new Vector2(-1, -1);
            Vector2 nextPoint = pointDefault;
            var tries = 0;

            while(tries < 4)
			{
                var currentDirection = WalkDirections[direction];
                nextPoint = adjacentPoints.Where(x => x == finalPoints[0] && (x - current).normalized == currentDirection).DefaultIfEmpty(pointDefault).First();
                if(nextPoint != pointDefault)
				{
                    break;
				}

                nextPoint = adjacentPoints.Where(x => !finalPoints.Contains(x) && (x - current).normalized == currentDirection).DefaultIfEmpty(pointDefault).First();
                if (nextPoint == pointDefault)
                {
                    direction = GetNextDirection(direction);
                    tries++;
                }
                else
				{
                    tries = 4;
                    break;
				}
            }
            /*
            nextPoint = adjacentPoints.Where(x => !finalPoints.Contains(x) && (x - current).normalized == Vector2.up).DefaultIfEmpty(pointDefault).First();
            if (nextPoint == pointDefault)
            {
                nextPoint = adjacentPoints.Where(x => !finalPoints.Contains(x) && (x - current).normalized == Vector2.right).DefaultIfEmpty(pointDefault).First();
            }
            if (nextPoint == pointDefault)
            {
                nextPoint = adjacentPoints.Where(x => !finalPoints.Contains(x) && (x - current).normalized == Vector2.down).DefaultIfEmpty(pointDefault).First();
            }
            if (nextPoint == pointDefault)
            {
                nextPoint = adjacentPoints.Where(x => !finalPoints.Contains(x) && (x - current).normalized == Vector2.left).DefaultIfEmpty(pointDefault).First();
             }
            */

            if(nextPoint == pointDefault || finalPoints.Contains(nextPoint))
			{
                done = true;
			}
			else
			{
                current = nextPoint;
                finalPoints.Add(nextPoint);
			}
        }

        var result = new List<Vector3>();
        finalPoints.ForEach(point => result.Add(new Vector3(point.x, 0, point.y)));
        return result;
	}

    private Vector3[] GetBoundsOfCluster(BiomeClusterNode node)
	{
        var tiles = node.tiles;
        
        var points = new HashSet<Vector2>();
        foreach(var tile in tiles)
		{
            points.Add(new Vector2(tile.x, tile.y));
            points.Add(new Vector2((tile.x + 1), tile.y));
            points.Add(new Vector2(tile.x, (tile.y + 1)));
            points.Add(new Vector2((tile.x + 1), (tile.y + 1)));
        }

        var id = 0;
        var nodes = new List<Node>();
        foreach(var point in points)
		{
            nodes.Add(new Node(point.x, point.y, id));
		}

        Hull.setConvexHull(nodes);
        var lines = Hull.setConcaveHull(0, 1);
        var finalPoints = new HashSet<Vector3>();

        foreach(var line in lines)
		{
            finalPoints.Add(new Vector3((float)line.nodes[0].x * tileSize, 0, (float)line.nodes[0].y * tileSize));
            finalPoints.Add(new Vector3((float)line.nodes[1].x * tileSize, 0, (float)line.nodes[1].y * tileSize));
        }

        return finalPoints.ToArray();

        //return WalkBounds(points.ToList()).ToArray();
	}

    private void GenerateBiomeClusters(List<BiomeClusterNode> clusters)
	{
        List<LocalProceduralBiome> proceduralBiomes = new List<LocalProceduralBiome>();

        foreach (var cluster in clusters)
		{
            LocalProceduralBiome biome = LocalProceduralBiome.CreateInstanceInScene(vMan);
            biome.dataMask = (BiomeDataMask)CombineAllBiomeMasks();
            biome.baseResolution = 1024;
            biome.falloffDistance = tileSize * 0.5f;
            biome.biomeMaskResolution = 512;
            biome.anchors = GetBoundsOfCluster(cluster);

            biome.terrainGraph = cluster.biome.biomeGraph;
            proceduralBiomes.Add(biome);
            if (biome.terrainGraph != null)
            {
                biome.CleanUp();
                biome.MarkChanged();
            }
        }

        vMan.Generate();
	}

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
