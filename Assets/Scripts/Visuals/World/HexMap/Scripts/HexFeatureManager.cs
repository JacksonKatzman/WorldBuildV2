using Game.Incidents;
using Game.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Terrain
{
    public class HexFeatureManager : MonoBehaviour
	{
		private static Dictionary<int, int> CurvedRiverRotationByPowers = new Dictionary<int, int>
		{
			{5, 0}, {10, 60}, {20, 120}, {40, 180}, {17, 240}, {34, 300}
		};

		private static Dictionary<int, int> SharpRiverRotationByPowers = new Dictionary<int, int>
		{
			{3, 0}, {6, 60}, {12, 120}, {24, 180}, {48, 240}, {33, 300}
		};

		public HexMesh walls;

		public AssetCollection assetCollection;
		public ConfigurableHexTerrain configurableHexTerrainPrefab;

		Transform container;

		public void Clear()
		{
			if (container)
			{
				if(Application.isEditor)
                {
					DestroyImmediate(container.gameObject);
				}
				else
                {
					Destroy(container.gameObject);
				}
			}
			container = new GameObject("Features Container").transform;
			container.SetParent(transform, false);
			walls.Clear();
		}

		public void Apply()
		{
			walls.Apply();
		}

		Transform PickPrefab(
			HexFeatureCollection[] collection,
			int level, float hash, float choice
		)
		{
			if (level > 0)
			{
				float[] thresholds = HexMetrics.GetFeatureThresholds(level - 1);
				for (int i = 0; i < thresholds.Length; i++)
				{
					if (hash < thresholds[i])
					{
						return collection[i].Pick(choice);
					}
				}
			}
			return null;
		}

		//i want to make a mountain builder that takes a collection, determines the cells that will have mountains,
		//the ones that will have foothills, then uses the existing placement of the edge vertices to build one that fits the area
		//and ideally perturbs it as well.

		public void AddBridge(Vector3 roadCenter1, Vector3 roadCenter2)
		{
			roadCenter1 = HexMetrics.Perturb(roadCenter1);
			roadCenter2 = HexMetrics.Perturb(roadCenter2);
			var bridgePrefab = SimRandom.RandomEntryFromArray(assetCollection.bridges);
			Transform instance = Instantiate(bridgePrefab);
			instance.localPosition = (roadCenter1 + roadCenter2) * 0.5f;
			instance.forward = roadCenter2 - roadCenter1;
			float length = Vector3.Distance(roadCenter1, roadCenter2);
			instance.localScale = new Vector3(
				1f, 1f, length * (1f / HexMetrics.bridgeDesignLength)
			);
			instance.SetParent(container, false);
		}

		public void AddFeature(HexCell cell, Vector3 position)
		{
			Transform prefab;
			HexHash hash = HexMetrics.SampleHashGrid(position);
			var perturbPrefabPosition = true;
			if (cell.HasLandmark && cell.LandmarkPositionAllocated == false)
			{
				SerializedObjectCollection collection = AssetService.Instance.objectData.collections[typeof(LandmarkPreset)];
				LandmarkPreset preset = collection.objects[cell.LandmarkType] as LandmarkPreset;
				prefab = SimRandom.RandomEntryFromList(preset.models);
				cell.LandmarkPositionAllocated = true;
				if(cell.LandmarkType == "Bare_Mountain")
                {
					perturbPrefabPosition = false;
                }
			}
			else
			{
				prefab = PickPrefab(
					assetCollection.urbanCollections, cell.UrbanLevel, hash.a, hash.d
				);
				Transform otherPrefab = PickPrefab(
					assetCollection.ruralCollections, cell.FarmLevel, hash.b, hash.d
				);
				float usedHash = hash.a;
				if (prefab)
				{
					if (otherPrefab && hash.b < hash.a)
					{
						prefab = otherPrefab;
						usedHash = hash.b;
					}
				}
				else if (otherPrefab)
				{
					prefab = otherPrefab;
					usedHash = hash.b;
				}
				otherPrefab = PickPrefab(
					assetCollection.plantCollections, cell.PlantLevel, hash.c, hash.d
				);
				if (prefab)
				{
					if (otherPrefab && hash.c < usedHash)
					{
						prefab = otherPrefab;
						usedHash = hash.c;
					}
				}
				else if (otherPrefab)
				{
					prefab = otherPrefab;
					usedHash = hash.c;
				}

				/*
				Transform mountainPrefab = PickPrefab(
					assetCollection.mountainCollections, cell.MountainLevel, hash.f, hash.d);
				if(prefab)
                {
					if(mountainPrefab && hash.f < usedHash)
                    {
						prefab = mountainPrefab;
						usedHash = hash.f;
                    }
                }
				else if(mountainPrefab)
                {
					prefab = mountainPrefab;
					usedHash = hash.f;
                }
				*/
				else
				{
					return;
				}
			}

			if (prefab)
			{
				Transform instance = Instantiate(prefab);
				position.y += instance.localScale.y * 0.5f;
				//instance.localPosition = HexMetrics.Perturb(position);
				if (perturbPrefabPosition)
				{
					instance.localPosition = HexMetrics.Perturb(position);
					instance.localRotation = Quaternion.Euler(0f, 360f * hash.e, 0f);
				}
				else
                {
					instance.localPosition = cell.Position;
					instance.localRotation = Quaternion.Euler(0f, 0f, 0f);
				}
				//instance.localRotation = Quaternion.Euler(0f, 360f * hash.e, 0f);
				instance.SetParent(container, false);
			}
		}

		public void AddHexFeature(HexCell cell, Vector3 position)
		{
			var biomeData = cell.BiomeData;
			var container = new AssetPositionInformationContainer();

			if(cell.Elevation - HexMetrics.globalWaterLevel < biomeData.hillThreshold)
            {
				AddFlatTerrain(cell, position, biomeData, ref container);
			}
			else
            {
				AddMountain(cell, position, biomeData, ref container);
            }

			if (biomeData.foliageAssets.Count > 0)
			{
				foreach (var placeholder in container.positionInformation)
				{
					var doodadPrefab = SimRandom.RandomEntryFromList(biomeData.foliageAssets);
					var doodad = Instantiate(doodadPrefab);
					doodad.transform.localPosition = placeholder.position;
					doodad.transform.SetParent(this.container, false);
				}
			}
		}

		public void AddMountain(HexCell cell, Vector3 position, BiomeData biomeData, ref AssetPositionInformationContainer container)
        {
			if(cell.HasIncomingRiver && cell.HasOutgoingRiver)
            {
				return;
            }

			Transform instance = null;
			if(cell.Elevation - HexMetrics.globalWaterLevel >= biomeData.mountainThreshold)
            {
				//mountain
				instance = cell.HasOutgoingRiver ? 
				Instantiate(SimRandom.RandomEntryFromList(biomeData.riverStartMountainAssets)).transform :
				Instantiate(SimRandom.RandomEntryFromList(biomeData.mountainAssets)).transform;
			}
			else
            {
				//hills
				instance = cell.HasOutgoingRiver ?
				Instantiate(SimRandom.RandomEntryFromList(biomeData.riverStartHillAssets)).transform :
				Instantiate(SimRandom.RandomEntryFromList(biomeData.hillAssets)).transform;
			}

			container.AddRange(instance.GetComponentsInChildren<AssetPlaceholder>());
			instance.localPosition = position;
			if (cell.HasOutgoingRiver)
			{
				instance.localRotation = Quaternion.Euler(0f, ((int)cell.OutgoingRiver) * 60.0f, 0f);
			}
			else
			{
				instance.localRotation = Quaternion.Euler(0f, SimRandom.RandomRange(0, 5) * 60.0f, 0f);
			}

			//instance.GetComponent<TerrainTextureUpdater>().UpdateTerrainTexture(biomeData.terrainMaterial);

			List<HexCell> smallerNeighbors = new List<HexCell>();
			for (Terrain.HexDirection d = Terrain.HexDirection.NE; d <= Terrain.HexDirection.NW; d++)
			{
				HexCell neighbor = cell.GetNeighbor(d);
				if(neighbor.Elevation < cell.Elevation)
                {
					smallerNeighbors.Add(neighbor);
                }
			}

			var finalNeighbors = new List<HexCell>();

			if(smallerNeighbors.Count == 0)
            {
				instance.localScale = new Vector3(1.4f, 1.4f, 1.4f);
			}
			else if(smallerNeighbors.Count == 3 && smallerNeighbors[1].IsNeighbor(smallerNeighbors[0]) && smallerNeighbors[1].IsNeighbor(smallerNeighbors[2]))
            {
				finalNeighbors.AddRange(smallerNeighbors);
            }
			else if(smallerNeighbors.Count == 2 && smallerNeighbors[0].IsNeighbor(smallerNeighbors[1]))
            {
				finalNeighbors.AddRange(smallerNeighbors);
			}
			else if(smallerNeighbors.Count == 1)
            {
				finalNeighbors.AddRange(smallerNeighbors);
			}

			if(finalNeighbors.Count > 0)
            {
				instance.localScale = new Vector3(1.35f, 1.35f, 1.35f);
			}

			foreach (var final in finalNeighbors)
			{
				var moveVector = new Vector3(final.Position.x - cell.Position.x, 0.0f, final.Position.z - cell.Position.z) / 10;
				instance.localPosition = instance.localPosition - moveVector;
			}

			instance.SetParent(this.container, false);
		}

		public void AddFlatTerrain(HexCell cell, Vector3 position, BiomeData biomeData, ref AssetPositionInformationContainer container)
        {
			var configurableHexTerrain = Instantiate(configurableHexTerrainPrefab);
			configurableHexTerrain.transform.localPosition = position;
			configurableHexTerrain.transform.SetParent(this.container, false);
			var rotation = 0;

			if (!cell.HasRiver)
			{

				for (Terrain.HexDirection d = Terrain.HexDirection.NE; d <= Terrain.HexDirection.NW; d++)
				{
					var degrees = (((int)d) * 60) + rotation;
					var template = SimRandom.RandomEntryFromList(configurableHexTerrain.roadOnlyBaseTemplates);
					template.transform.localRotation = Quaternion.Euler(0f, degrees, 0f);
					container.AddRange(template.GetComponentsInChildren<AssetPlaceholder>(true));

					if (!cell.HasRoadThroughEdge(d))
					{
						template = SimRandom.RandomEntryFromList(configurableHexTerrain.roadOnlyRoadTemplates);
						template.transform.localRotation = Quaternion.Euler(0f, degrees, 0f);
						container.AddRange(template.GetComponentsInChildren<AssetPlaceholder>(true));

						if (cell.GetNeighbor(d) != null && cell.GetNeighbor(d).Elevation == cell.Elevation && (d < Terrain.HexDirection.SW))
						{
							template = SimRandom.RandomEntryFromList(configurableHexTerrain.fullOffTemplates);
							template.transform.localRotation = Quaternion.Euler(0f, degrees, 0f);
							container.AddRange(template.GetComponentsInChildren<AssetPlaceholder>(true));
						}
					}
					else
					{
						if (cell.GetNeighbor(d) != null && cell.GetNeighbor(d).Elevation == cell.Elevation && (d < Terrain.HexDirection.SW))
						{
							template = SimRandom.RandomEntryFromList(configurableHexTerrain.roadOnlyOffTemplates);
							template.transform.localRotation = Quaternion.Euler(0f, degrees, 0f);
							container.AddRange(template.GetComponentsInChildren<AssetPlaceholder>(true));
						}
					}
				}

			}
			else if (cell.HasIncomingRiver && cell.HasOutgoingRiver)
			{
				var riverOrientation = Mathf.Abs(((int)cell.IncomingRiver) - ((int)cell.OutgoingRiver));
				var rot = Mathf.Abs(((int)Terrain.HexDirection.NE) - ((int)cell.IncomingRiver));
				rotation = rot * 60;
				if (riverOrientation == 3)
				{
					//straight
					var template = cell.HasRoads ? SimRandom.RandomEntryFromList(configurableHexTerrain.riverAndRoadsStraightTemplates) :
						SimRandom.RandomEntryFromList(configurableHexTerrain.riverOnlyStraightTemplates);
					template.transform.localRotation = Quaternion.Euler(0f, rotation, 0f);
					container.AddRange(template.GetComponentsInChildren<AssetPlaceholder>(true));
				}
				else if (riverOrientation == 2 || riverOrientation == 4)
				{
					//curved
					var powerKey = (int)(Mathf.Pow(2, (int)cell.IncomingRiver) + Mathf.Pow(2, (int)cell.OutgoingRiver));
					rotation = CurvedRiverRotationByPowers[powerKey];
					var template = cell.HasRoads ? SimRandom.RandomEntryFromList(configurableHexTerrain.riverAndRoadsCurvedTemplates) :
						SimRandom.RandomEntryFromList(configurableHexTerrain.riverOnlyCurvedTemplates);
					template.transform.localRotation = Quaternion.Euler(0f, rotation, 0f);
					container.AddRange(template.GetComponentsInChildren<AssetPlaceholder>(true));
				}
				else
				{
					//sharp
					var powerKey = (int)(Mathf.Pow(2, (int)cell.IncomingRiver) + Mathf.Pow(2, (int)cell.OutgoingRiver));
					rotation = SharpRiverRotationByPowers[powerKey];
					var template = cell.HasRoads ? SimRandom.RandomEntryFromList(configurableHexTerrain.riverAndRoadsSharpTemplates) :
						SimRandom.RandomEntryFromList(configurableHexTerrain.riverOnlySharpTemplates);
					template.transform.localRotation = Quaternion.Euler(0f, rotation, 0f);
					container.AddRange(template.GetComponentsInChildren<AssetPlaceholder>(true));
				}

				//handle the offs here

				for (Terrain.HexDirection d = Terrain.HexDirection.NE; d <= Terrain.HexDirection.SE; d++)
				{
					if (cell.GetNeighbor(d) != null && cell.GetNeighbor(d).Elevation == cell.Elevation)
					{
						var degrees = Mathf.Abs(0 - (int)d) * 60;

						if (cell.IncomingRiver == d || cell.OutgoingRiver == d)
						{
							var template = SimRandom.RandomEntryFromList(configurableHexTerrain.riverOnlyOffTemplates);
							template.transform.localRotation = Quaternion.Euler(0f, degrees, 0f);
							container.AddRange(template.GetComponentsInChildren<AssetPlaceholder>(true));
						}
						else if (cell.HasRoadThroughEdge(d))
						{
							var template = SimRandom.RandomEntryFromList(configurableHexTerrain.roadOnlyOffTemplates);
							template.transform.localRotation = Quaternion.Euler(0f, degrees, 0f);
							container.AddRange(template.GetComponentsInChildren<AssetPlaceholder>(true));
						}
						else
						{
							var template = SimRandom.RandomEntryFromList(configurableHexTerrain.fullOffTemplates);
							template.transform.localRotation = Quaternion.Euler(0f, degrees, 0f);
							container.AddRange(template.GetComponentsInChildren<AssetPlaceholder>(true));
						}
					}
				}
			}	
		}

		public void AddSpecialFeature(HexCell cell, Vector3 position)
		{
			if (!string.IsNullOrEmpty(cell.LandmarkType))
			{
				HexHash hash = HexMetrics.SampleHashGrid(position);
				//make a way to set the landmark type in the cell itself
				//var landmarkPrefabList = assetCollection.landmarkCollections[cell.LandmarkType];
				SerializedObjectCollection collection = AssetService.Instance.objectData.collections[typeof(LandmarkPreset)];
				LandmarkPreset preset = collection.objects[cell.LandmarkType] as LandmarkPreset;
				var model = SimRandom.RandomEntryFromList(preset.models);
				//var landmarkPrefab = SimRandom.RandomEntryFromList(landmarkPrefabList);
				Transform instance = Instantiate(model);
				instance.localPosition = HexMetrics.Perturb(position);
				instance.localRotation = Quaternion.Euler(0f, 360f * hash.e, 0f);
				instance.SetParent(container, false);
			}
		}

		public void AddWall(
			EdgeVertices near, HexCell nearCell,
			EdgeVertices far, HexCell farCell,
			bool hasRiver, bool hasRoad
		)
		{
			if (
				nearCell.Walled != farCell.Walled &&
				!nearCell.IsUnderwater && !farCell.IsUnderwater &&
				nearCell.GetEdgeType(farCell) != HexEdgeType.Cliff
			)
			{
				AddWallSegment(near.v1, far.v1, near.v2, far.v2);
				if (hasRiver || hasRoad)
				{
					AddWallCap(near.v2, far.v2);
					AddWallCap(far.v4, near.v4);
				}
				else
				{
					AddWallSegment(near.v2, far.v2, near.v3, far.v3);
					AddWallSegment(near.v3, far.v3, near.v4, far.v4);
				}
				AddWallSegment(near.v4, far.v4, near.v5, far.v5);
			}
		}

		public void AddWall(
			Vector3 c1, HexCell cell1,
			Vector3 c2, HexCell cell2,
			Vector3 c3, HexCell cell3
		)
		{
			if (cell1.Walled)
			{
				if (cell2.Walled)
				{
					if (!cell3.Walled)
					{
						AddWallSegment(c3, cell3, c1, cell1, c2, cell2);
					}
				}
				else if (cell3.Walled)
				{
					AddWallSegment(c2, cell2, c3, cell3, c1, cell1);
				}
				else
				{
					AddWallSegment(c1, cell1, c2, cell2, c3, cell3);
				}
			}
			else if (cell2.Walled)
			{
				if (cell3.Walled)
				{
					AddWallSegment(c1, cell1, c2, cell2, c3, cell3);
				}
				else
				{
					AddWallSegment(c2, cell2, c3, cell3, c1, cell1);
				}
			}
			else if (cell3.Walled)
			{
				AddWallSegment(c3, cell3, c1, cell1, c2, cell2);
			}
		}

		void AddWallSegment(
			Vector3 nearLeft, Vector3 farLeft, Vector3 nearRight, Vector3 farRight,
			bool addTower = false
		)
		{
			nearLeft = HexMetrics.Perturb(nearLeft);
			farLeft = HexMetrics.Perturb(farLeft);
			nearRight = HexMetrics.Perturb(nearRight);
			farRight = HexMetrics.Perturb(farRight);

			Vector3 left = HexMetrics.WallLerp(nearLeft, farLeft);
			Vector3 right = HexMetrics.WallLerp(nearRight, farRight);

			Vector3 leftThicknessOffset =
				HexMetrics.WallThicknessOffset(nearLeft, farLeft);
			Vector3 rightThicknessOffset =
				HexMetrics.WallThicknessOffset(nearRight, farRight);

			float leftTop = left.y + HexMetrics.wallHeight;
			float rightTop = right.y + HexMetrics.wallHeight;

			Vector3 v1, v2, v3, v4;
			v1 = v3 = left - leftThicknessOffset;
			v2 = v4 = right - rightThicknessOffset;
			v3.y = leftTop;
			v4.y = rightTop;
			walls.AddQuadUnperturbed(v1, v2, v3, v4);

			Vector3 t1 = v3, t2 = v4;

			v1 = v3 = left + leftThicknessOffset;
			v2 = v4 = right + rightThicknessOffset;
			v3.y = leftTop;
			v4.y = rightTop;
			walls.AddQuadUnperturbed(v2, v1, v4, v3);

			walls.AddQuadUnperturbed(t1, t2, v3, v4);

			if (addTower)
			{
				Transform towerInstance = Instantiate(assetCollection.wallTower);
				towerInstance.transform.localPosition = (left + right) * 0.5f;
				Vector3 rightDirection = right - left;
				rightDirection.y = 0f;
				towerInstance.transform.right = rightDirection;
				towerInstance.SetParent(container, false);
			}
		}

		void AddWallSegment(
			Vector3 pivot, HexCell pivotCell,
			Vector3 left, HexCell leftCell,
			Vector3 right, HexCell rightCell
		)
		{
			if (pivotCell.IsUnderwater)
			{
				return;
			}

			bool hasLeftWall = !leftCell.IsUnderwater &&
				pivotCell.GetEdgeType(leftCell) != HexEdgeType.Cliff;
			bool hasRighWall = !rightCell.IsUnderwater &&
				pivotCell.GetEdgeType(rightCell) != HexEdgeType.Cliff;

			if (hasLeftWall)
			{
				if (hasRighWall)
				{
					bool hasTower = false;
					if (leftCell.Elevation == rightCell.Elevation)
					{
						HexHash hash = HexMetrics.SampleHashGrid(
							(pivot + left + right) * (1f / 3f)
						);
						hasTower = hash.e < HexMetrics.wallTowerThreshold;
					}
					AddWallSegment(pivot, left, pivot, right, hasTower);
				}
				else if (leftCell.Elevation < rightCell.Elevation)
				{
					AddWallWedge(pivot, left, right);
				}
				else
				{
					AddWallCap(pivot, left);
				}
			}
			else if (hasRighWall)
			{
				if (rightCell.Elevation < leftCell.Elevation)
				{
					AddWallWedge(right, pivot, left);
				}
				else
				{
					AddWallCap(right, pivot);
				}
			}
		}

		void AddWallCap(Vector3 near, Vector3 far)
		{
			near = HexMetrics.Perturb(near);
			far = HexMetrics.Perturb(far);

			Vector3 center = HexMetrics.WallLerp(near, far);
			Vector3 thickness = HexMetrics.WallThicknessOffset(near, far);

			Vector3 v1, v2, v3, v4;

			v1 = v3 = center - thickness;
			v2 = v4 = center + thickness;
			v3.y = v4.y = center.y + HexMetrics.wallHeight;
			walls.AddQuadUnperturbed(v1, v2, v3, v4);
		}

		void AddWallWedge(Vector3 near, Vector3 far, Vector3 point)
		{
			near = HexMetrics.Perturb(near);
			far = HexMetrics.Perturb(far);
			point = HexMetrics.Perturb(point);

			Vector3 center = HexMetrics.WallLerp(near, far);
			Vector3 thickness = HexMetrics.WallThicknessOffset(near, far);

			Vector3 v1, v2, v3, v4;
			Vector3 pointTop = point;
			point.y = center.y;

			v1 = v3 = center - thickness;
			v2 = v4 = center + thickness;
			v3.y = v4.y = pointTop.y = center.y + HexMetrics.wallHeight;

			walls.AddQuadUnperturbed(v1, point, v3, pointTop);
			walls.AddQuadUnperturbed(point, v2, pointTop, v4);
			walls.AddTriangleUnperturbed(pointTop, v3, v4);
		}
	}
}