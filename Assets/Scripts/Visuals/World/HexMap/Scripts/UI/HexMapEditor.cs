using Game.Enums;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Terrain
{
	public class HexMapEditor : MonoBehaviour
	{

		public HexGrid hexGrid;

		public Material terrainMaterial;

		int activeElevation;
		int activeWaterLevel;

		int activeUrbanLevel, activeFarmLevel, activePlantLevel, activeSpecialIndex;

		BiomeTerrainType activeTerrainTypeIndex;

		int brushSize;

		bool applyElevation = false;
		bool applyWaterLevel = false;

		bool applyUrbanLevel, applyFarmLevel, applyPlantLevel, applySpecialIndex;

		enum OptionalToggle
		{
			Ignore, Yes, No
		}

		OptionalToggle riverMode, roadMode, walledMode;

		bool isDrag;
		HexDirection dragDirection;
		HexCell previousCell;

		public void SetTerrainTypeIndex(int index)
		{
			activeTerrainTypeIndex = (BiomeTerrainType)index;
		}

		public void SetApplyElevation(bool toggle)
		{
			applyElevation = toggle;
		}

		public void SetElevation(float elevation)
		{
			activeElevation = (int)elevation;
		}

		public void SetApplyWaterLevel(bool toggle)
		{
			applyWaterLevel = toggle;
		}

		public void SetWaterLevel(float level)
		{
			activeWaterLevel = (int)level;
		}

		public void SetApplyUrbanLevel(bool toggle)
		{
			applyUrbanLevel = toggle;
		}

		public void SetUrbanLevel(float level)
		{
			activeUrbanLevel = (int)level;
		}

		public void SetApplyFarmLevel(bool toggle)
		{
			applyFarmLevel = toggle;
		}

		public void SetFarmLevel(float level)
		{
			activeFarmLevel = (int)level;
		}

		public void SetApplyPlantLevel(bool toggle)
		{
			applyPlantLevel = toggle;
		}

		public void SetPlantLevel(float level)
		{
			activePlantLevel = (int)level;
		}

		public void SetApplySpecialIndex(bool toggle)
		{
			applySpecialIndex = toggle;
		}

		public void SetSpecialIndex(float index)
		{
			activeSpecialIndex = (int)index;
		}

		public void SetBrushSize(float size)
		{
			brushSize = (int)size;
		}

		public void SetRiverMode(int mode)
		{
			riverMode = (OptionalToggle)mode;
		}

		public void SetRoadMode(int mode)
		{
			roadMode = (OptionalToggle)mode;
		}

		public void SetWalledMode(int mode)
		{
			walledMode = (OptionalToggle)mode;
		}

		public void SetEditMode(bool toggle)
		{
			enabled = toggle;
		}

		public void ShowGrid(bool visible)
		{
			if (visible)
			{
				terrainMaterial.EnableKeyword("GRID_ON");
			}
			else
			{
				terrainMaterial.DisableKeyword("GRID_ON");
			}
		}

		void Awake()
		{
			terrainMaterial.DisableKeyword("GRID_ON");
			Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
			SetEditMode(false);
		}

		void Update()
		{
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				if (Input.GetMouseButton(0))
				{
					HandleInput();
					return;
				}
				if (Input.GetKeyDown(KeyCode.U))
				{
					if (Input.GetKey(KeyCode.LeftShift))
					{
						DestroyUnit();
					}
					else
					{
						CreateUnit();
					}
					return;
				}
			}
			previousCell = null;
		}

		HexCell GetCellUnderCursor()
		{
			return
				hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		}

		void CreateUnit()
		{
			HexCell cell = GetCellUnderCursor();
			if (cell && !cell.Unit)
			{
				hexGrid.AddUnit(
					Instantiate(HexUnit.unitPrefab), cell, Random.Range(0f, 360f)
				);
			}
		}

		void DestroyUnit()
		{
			HexCell cell = GetCellUnderCursor();
			if (cell && cell.Unit)
			{
				hexGrid.RemoveUnit(cell.Unit);
			}
		}

		void HandleInput()
		{
			HexCell currentCell = GetCellUnderCursor();
			if (currentCell)
			{
				if (previousCell && previousCell != currentCell)
				{
					ValidateDrag(currentCell);
				}
				else
				{
					isDrag = false;
				}
				EditCells(currentCell);
				previousCell = currentCell;
			}
			else
			{
				previousCell = null;
			}
		}

		void ValidateDrag(HexCell currentCell)
		{
			for (
				dragDirection = HexDirection.NE;
				dragDirection <= HexDirection.NW;
				dragDirection++
			)
			{
				if (previousCell.GetNeighbor(dragDirection) == currentCell)
				{
					isDrag = true;
					return;
				}
			}
			isDrag = false;
		}

		void EditCells(HexCell center)
		{
			int centerX = center.coordinates.X;
			int centerZ = center.coordinates.Z;

			for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
			{
				for (int x = centerX - r; x <= centerX + brushSize; x++)
				{
					EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
				}
			}
			for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
			{
				for (int x = centerX - brushSize; x <= centerX + r; x++)
				{
					EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
				}
			}
		}

		void EditCell(HexCell cell)
		{
			if (cell)
			{
				if (activeTerrainTypeIndex >= 0)
				{
					cell.TerrainType = activeTerrainTypeIndex;
				}
				if (applyElevation)
				{
					cell.Elevation = activeElevation;
				}
				if (applyWaterLevel)
				{
					cell.WaterLevel = activeWaterLevel;
				}
				if (applySpecialIndex)
				{
					//need to later replace with way to get presets from asset service
					//cell.LandmarkType = (LandmarkType)activeSpecialIndex;
				}
				if (applyUrbanLevel)
				{
					cell.UrbanLevel = activeUrbanLevel;
				}
				if (applyFarmLevel)
				{
					cell.FarmLevel = activeFarmLevel;
				}
				if (riverMode == OptionalToggle.No)
				{
					cell.RemoveRiver();
				}
				if (roadMode == OptionalToggle.No)
				{
					cell.RemoveRoads();
				}
				if (walledMode != OptionalToggle.Ignore)
				{
					cell.Walled = walledMode == OptionalToggle.Yes;
				}
				if (isDrag)
				{
					HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
					if (otherCell)
					{
						if (riverMode == OptionalToggle.Yes)
						{
							otherCell.SetOutgoingRiver(dragDirection);
						}
						if (roadMode == OptionalToggle.Yes)
						{
							otherCell.AddRoad(dragDirection);
						}
					}
				}
			}
		}
	}
}