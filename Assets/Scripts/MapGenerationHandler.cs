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
using Game.Generators.Noise;

public class MapGenerationHandler : MonoBehaviour
{
    private static Dictionary<int, Vector2Int> WalkDirections = new Dictionary<int, Vector2Int>
    {
        {0, new Vector2Int(1, 0) },
        {1, new Vector2Int(0, -1) },
        {2, new Vector2Int(-1, 0) },
        {3, new Vector2Int(0, 1) }
    };

    public VistaManager vMan;
    public BiomeContainer biomes;
    public AnimationCurve temperatureCurve;

    public NoiseSettings noiseSettings;

    public int tileSize;

    public Renderer mapRenderer;

    private bool done;

    void Start()
    {
        GenerateByCluster();
    }

    public void DrawMap()
	{
        /*
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
        */
        mapRenderer.sharedMaterial.mainTexture = NoiseGenerator.GenerateARGBNoiseTexture(noiseSettings);
        mapRenderer.transform.localScale = new Vector3(noiseSettings.worldSize.x, 1, noiseSettings.worldSize.y);
    }

    public void DrawMap(float[,] heightMap)
    {
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

    private void Update()
	{
        if (Input.GetKey(KeyCode.Escape))
        {
            done = true;
        }
    }

    private void GenerateByCluster()
	{
        /*
        var desiredSize = 10;//noiseVector.x;
        var heightMap = NoiseGenerator.GeneratePerlinNoise(noiseVector, noiseScale,
                                                         octaves, persistence,
                                                          lacunarity, offset);
        var rainfallMap = NoiseGenerator.GeneratePerlinNoise(noiseVector, noiseScale,
                                                         octaves, persistence,
                                                          lacunarity, offset);
        var fertilityMap = NoiseGenerator.GeneratePerlinNoise(noiseVector, noiseScale,
                                                         octaves, persistence,
                                                          lacunarity, offset);

        DrawMap(heightMap);

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
        */
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

                var possibleClusters = clusterList.Where(x => x.biome == biome && x.tiles.Intersect(neighborList).ToList().Count > 0).ToList();

                if(possibleClusters.Count > 0)
				{
                    var chosenCluster = possibleClusters[0];
                    if (possibleClusters.Count > 1)
                    {
                        for (var c = 1; c < possibleClusters.Count; c++)
                        {
                            chosenCluster.tiles.AddRange(possibleClusters[c].tiles);
                            clusterList.Remove(possibleClusters[c]);
                        }
                    }
                    chosenCluster.tiles.Add(new Vector2Int(x, z));
				}
				else
				{
                    clusterList.Add(new BiomeClusterNode(biome, new Vector2Int(x, z)));
                }
                /*
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
                */

                neighborList.Clear();
            }
        }

        return clusterList;
    }

    private int GetNextDirection(int current, int mod)
	{
        int remainder = (current + mod) % 4;
        return (remainder < 0) ? (4 + remainder) : remainder;
	}

    private void AddPointsOnEdges(HashSet<Vector2> points, List<Vector2Int> tiles, Vector2Int currentTile)
	{
        /*
        if(!tiles.Contains(new Vector2Int(currentTile.x, currentTile.y - 1)))
		{
            points.Add(new Vector2((currentTile.x + 1) * tileSize, currentTile.y * tileSize));
            points.Add(new Vector2(currentTile.x * tileSize, currentTile.y * tileSize));
		}
        if (!tiles.Contains(new Vector2Int(currentTile.x - 1, currentTile.y)))
        {
            points.Add(new Vector2(currentTile.x * tileSize, currentTile.y * tileSize));
            points.Add(new Vector2(currentTile.x * tileSize, (currentTile.y + 1) * tileSize));
        }
        if (!tiles.Contains(new Vector2Int(currentTile.x, currentTile.y + 1)))
        {
            points.Add(new Vector2((currentTile.x) * tileSize, (currentTile.y + 1) * tileSize));
            points.Add(new Vector2((currentTile.x + 1) * tileSize, (currentTile.y + 1) * tileSize));
        }
        if (!tiles.Contains(new Vector2Int(currentTile.x + 1, currentTile.y)))
        {
            points.Add(new Vector2((currentTile.x + 1) * tileSize, (currentTile.y + 1) * tileSize));
            points.Add(new Vector2((currentTile.x + 1) * tileSize, currentTile.y * tileSize));
        }
        */
        if (!tiles.Contains(currentTile + WalkDirections[0]))
        {
            points.Add((currentTile + WalkDirections[0]) * tileSize);
            points.Add((currentTile + WalkDirections[0] + WalkDirections[3]) * tileSize);
        }
        if (!tiles.Contains(currentTile + WalkDirections[3]))
        {
            points.Add((currentTile + WalkDirections[0] + WalkDirections[3]) * tileSize);
            points.Add((currentTile + WalkDirections[3]) * tileSize);
        }
        if (!tiles.Contains(currentTile + WalkDirections[2]))
        {
            points.Add((currentTile + WalkDirections[3]) * tileSize);
            points.Add(currentTile * tileSize);
        }
        if (!tiles.Contains(currentTile + WalkDirections[1]))
        {
            points.Add(currentTile * tileSize);
            points.Add((currentTile + WalkDirections[0]) * tileSize);
        }
    }

    private List<Vector2> CullExtraPoints(List<Vector2> points)
	{
        var necessaryPoints = new List<Vector2>();
        if(points[1] - points[0] != points[0] - points[points.Count-1])
		{
            necessaryPoints.Add(points[0]);
		}
        for(int x = 1; x < points.Count - 1; x++)
		{
            if(points[x+1] - points[x] != points[x] - points[x-1])
			{
                necessaryPoints.Add(points[x]);
			}
		}
        if (points[0] - points[points.Count-1] != points[points.Count-1] - points[points.Count - 2])
        {
            necessaryPoints.Add(points[0]);
        }
        return necessaryPoints;
    }

    private List<Vector3> WalkBounds(BiomeClusterNode node)
	{
        var tiles = node.tiles;
        var currentDirection = 0;
        done = false;
        var currentTile = tiles[0];
        var firstTile = tiles[0];
        var visitedTiles = new List<Vector2Int>();

        var boundaryPoints = new HashSet<Vector2>();

        AddPointsOnEdges(boundaryPoints, tiles, currentTile);

        if (tiles.Count == 1)
        {
            done = true;
        }

        while (!done)
		{
            var preferredDirection = GetNextDirection(currentDirection, 1);
            var preferredTile = currentTile + WalkDirections[preferredDirection];
            var preferredTileExists = tiles.Contains(preferredTile);
            var preferredTileVisited = preferredTileExists ? visitedTiles.Contains(currentTile + WalkDirections[preferredDirection]) : true;
            var directionalTile = currentTile + WalkDirections[currentDirection];
            var directionalTileExists = tiles.Contains(directionalTile);
            var turns = 0;

            if(preferredTileExists && !preferredTileVisited)
			{
                visitedTiles.Add(currentTile);
                currentTile = preferredTile;
                currentDirection = preferredDirection;
                AddPointsOnEdges(boundaryPoints, tiles, currentTile);
			}
            else if(directionalTileExists)
			{
                if(currentTile + WalkDirections[currentDirection] == firstTile)
				{
                    done = true;
                    continue;
				}
                else
				{
                    visitedTiles.Add(currentTile);
                    currentTile = directionalTile;
                    AddPointsOnEdges(boundaryPoints, tiles, currentTile);
                }
			}
			else
			{
                currentDirection = GetNextDirection(currentDirection, -1);
                turns++;
                if(turns > 3)
				{
                    done = true;
				}
			}

            if(visitedTiles.Count > tiles.Count)
			{
                done = true;
			}
		}

        var finalPoints = boundaryPoints.ToList();

        if (finalPoints.Count > 4)
        {
            finalPoints = CullExtraPoints(boundaryPoints.ToList());
        }

        var bounds = new List<Vector3>();
        foreach (var point in finalPoints)
        {
            bounds.Add(new Vector3(point.x, 0, point.y));
        }

        return bounds;
    }

    private Vector3[] GetBoundsOfCluster(BiomeClusterNode node)
	{
        /*
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
        var lines = Hull.setConcaveHull(-1, 1);
        var finalPoints = new HashSet<Vector3>();

        foreach(var line in lines)
		{
            finalPoints.Add(new Vector3((float)line.nodes[0].x * tileSize, 0, (float)line.nodes[0].y * tileSize));
            finalPoints.Add(new Vector3((float)line.nodes[1].x * tileSize, 0, (float)line.nodes[1].y * tileSize));
        }

        return finalPoints.ToArray();
        */
        return WalkBounds(node).ToArray();
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
