using Game.WorldGeneration;
using System.Collections.Generic;
using Game.Enums;
using UnityEngine;
using System.Linq;

namespace Game.Visuals
{
	public class WorldVisualBuilder : MonoBehaviour
	{
		World world;
		TileVisual[,] terrainTiles;
		List<TileVisual> unsortedTiles;

		[SerializeField]
		AnimationCurve heightCurve;
		public void BuildWorld(World world)
		{
			this.world = world;

			BuildTerrain();
		}

		public void UpdateVisuals()
		{
			foreach (var tile in terrainTiles)
			{
				tile.UpdateVisuals();
			}
		}

		private void BuildTerrain()
		{
			var terrainNoiseMap = world.noiseMaps[MapCategory.TERRAIN];
			var terrainMapWidth = terrainNoiseMap.GetLength(0);
			var terrainMapHeight = terrainNoiseMap.GetLength(1);
			terrainTiles = new TileVisual[terrainMapWidth, terrainMapHeight];
			unsortedTiles = new List<TileVisual>();

			foreach(var chunk in world.worldChunks)
			{
				foreach(var tile in chunk.chunkTiles)
				{
					var worldPos = tile.GetWorldPosition();
					var height = terrainNoiseMap[worldPos.x, worldPos.y];

					var prefab = SimRandom.RandomEntryFromList(tile.biome.tilePrefab);
					var prefabWidth = prefab.GetComponent<TileVisual>().Renderer.bounds.size.x;
					var heightMultiplier = heightCurve.Evaluate(height) * 30;

					var terrainTile = terrainTiles[worldPos.x, worldPos.y] = Instantiate(prefab, 
						new Vector3((worldPos.x - terrainMapWidth/2) * prefabWidth, height * heightMultiplier, (worldPos.y - terrainMapHeight/2) * prefabWidth),
						Quaternion.identity, transform).GetComponent<TileVisual>();

					unsortedTiles.Add(terrainTile);
					terrainTile.tile = tile;
					terrainTile.Initialize();
				}
			}
		}
	}
}