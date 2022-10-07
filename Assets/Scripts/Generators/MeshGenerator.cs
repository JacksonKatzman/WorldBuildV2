using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Generators
{
	public static class MeshGenerator
	{
		public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve)
		{
			int width = heightMap.GetLength(0);
			int height = heightMap.GetLength(1);
			float topLeftX = (width - 1) / -2f;
			float topLeftZ = (height - 1) / 2f;

			MeshData meshData = new MeshData(width*2, height*2);
			int vertexIndex = 0;

			for(int y = 0; y < height; y++)
			{
				for(int x = 0; x < width; x++)
				{
					meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);
					meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

					if (x < width - 1 && y < height - 1)
					{
						meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
						meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
					}

					vertexIndex++;	
				}
			}

			return meshData;
		}

		public static VoxelMeshData GenerateVoxelTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve)
		{
			int width = heightMap.GetLength(0);
			int height = heightMap.GetLength(1);
			float topLeftX = ((width - 1) / -2f) - 0.5f;
			float topLeftZ = ((height - 1) / 2f) + 0.5f;

			VoxelMeshData meshData = new VoxelMeshData(width*2, height*2);
			int vertexIndex = 0;

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);
					meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

					meshData.vertices[vertexIndex + 1] = new Vector3(topLeftX + x + 1, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);
					meshData.uvs[vertexIndex + 1] = new Vector2((x+0) / (float)width, y / (float)height);

					meshData.vertices[vertexIndex + 2] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y - 1);
					meshData.uvs[vertexIndex + 2] = new Vector2(x / (float)width, (y+0) / (float)height);

					meshData.vertices[vertexIndex + 3] = new Vector3(topLeftX + x + 1, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y - 1);
					meshData.uvs[vertexIndex + 3] = new Vector2((x+0) / (float)width, (y+0) / (float)height);

					meshData.AddTriangle(vertexIndex, vertexIndex + 3, vertexIndex + 2);
					meshData.AddTriangle(vertexIndex + 3, vertexIndex, vertexIndex + 1);

					if (x < width - 1)
					{
						meshData.AddTriangle(vertexIndex + 1, vertexIndex + 6, vertexIndex + 3);
						meshData.AddTriangle(vertexIndex + 6, vertexIndex + 1, vertexIndex + 4);
					}

					if(y < height - 1)
					{
						meshData.AddTriangle(vertexIndex + 2, vertexIndex + (4 * height) + 1, vertexIndex + (4 * height));
						meshData.AddTriangle(vertexIndex + (4 * height) + 1, vertexIndex + 2, vertexIndex + 3);
					}

					vertexIndex += 4;
				}
			}

			return meshData;

		}
	}

	public class VoxelMeshData
	{
		public Vector3[] vertices;
		//public int[] triangles;
		public Vector2[] uvs;

		private List<int> tempTriangles;

		int triangleIndex;

		public VoxelMeshData(int meshWidth, int meshHeight)
		{
			vertices = new Vector3[meshWidth * meshHeight];
			//triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
			uvs = new Vector2[meshWidth * meshHeight];
			tempTriangles = new List<int>();
			var triCountGoal = ((meshWidth - 1) * (meshHeight - 1)) - ((meshWidth / 2 - 1) * (meshHeight / 2 - 1));
			triCountGoal *= 6;
			OutputLogger.LogFormat("GOAL TRIS: {0}", Enums.LogSource.IMPORTANT, triCountGoal);
		}

		public void AddTriangle(int a, int b, int c)
		{
			tempTriangles.Add(a);
			tempTriangles.Add(b);
			tempTriangles.Add(c);

			triangleIndex += 3;
		}

		public Mesh CreateMesh()
		{
			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.triangles = tempTriangles.ToArray();
			mesh.uv = uvs;
			mesh.RecalculateNormals();
			OutputLogger.LogFormat("ACHIEVED TRIS: {0}", Enums.LogSource.IMPORTANT, tempTriangles.Count);
			return mesh;
		}
	}

	public class MeshData
	{
		public Vector3[] vertices;
		public int[] triangles;
		public Vector2[] uvs;

		int triangleIndex;

		public MeshData(int meshWidth, int meshHeight)
		{
			vertices = new Vector3[meshWidth * meshHeight];
			triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
			uvs = new Vector2[meshWidth * meshHeight];
		}

		public void AddTriangle(int a, int b, int c)
		{
			triangles[triangleIndex] = a;
			triangles[triangleIndex + 1] = b;
			triangles[triangleIndex + 2] = c;
			triangleIndex += 3;
		}

		public Mesh CreateMesh()
		{
			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uvs;
			mesh.RecalculateNormals();
			return mesh;
		}
	}
}