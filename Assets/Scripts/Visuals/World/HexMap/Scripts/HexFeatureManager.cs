using Game.Incidents;
using Game.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Terrain
{
	public class HexFeatureManager : MonoBehaviour
	{
		public HexMesh walls;

		public AssetCollection assetCollection;

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
			//if rivers AND roads, use the blank configurable type, pass it info about what lines are taken, and fill in the gaps
			//else if mountain, grab mountains
			//same with hills
			//the tiles will all have to handle landmarks and cities as well.
			//Transform instance = Instantiate(AssetService.Instance.basePrefab);
			//instance.localPosition = position;
			//instance.SetParent(container, false);
			var biomeData = AssetService.Instance.BiomeDataContainer.GetBiomeData(cell.BiomeSubtype);
			if(biomeData.hillThreshold > cell.Elevation - HexMetrics.globalWaterLevel && !cell.HasRiver)
            {
				var assetContainer = biomeData.hexConfigurationAssetContainer;
				if(assetContainer != null)
                {
					var instance = Instantiate(assetContainer.roadsOnly[0]);
					instance.transform.localPosition = position;
					instance.transform.SetParent(container, false);

					var configurableHexTerrain = instance.GetComponent<ConfigurableHexTerrain>();
					foreach(var placeholder in configurableHexTerrain.placeholders)
                    {
						var doodadPrefab = SimRandom.RandomEntryFromList(biomeData.foliageAssets);
						var doodad = Instantiate(doodadPrefab);
						doodad.transform.localPosition = placeholder.transform.position;
						doodad.transform.SetParent(container, false);
					}
				}
			}
		}

		public void AddMountain(HexCell cell, Vector3 position)
        {
			if(cell.HasRiver)
            {
				return;
            }
			Transform instance = Instantiate(AssetService.Instance.testMountain);
			instance.localPosition = position;
			instance.localRotation = Quaternion.Euler(0f, SimRandom.RandomRange(0,5) * 60.0f, 0f);
			//instance.localScale = new Vector3(1.5f, 1.5f, 1.5f);
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

			instance.SetParent(container, false);
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