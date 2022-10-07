﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Visuals.Hex
{
	public class HexGameUI : MonoBehaviour
	{
		private static int SPEED = 24;

		public HexGrid grid;

		[SerializeField]
		private Camera mainCamera;

		HexCell currentCell;
		HexUnit selectedUnit;

		private void Start()
		{
			SetEditMode(false);
		}

		public void SetEditMode(bool toggle)
		{
			enabled = !toggle;
			grid.ShowUI(!toggle);
			grid.ClearPath();

			if (toggle)
			{
				Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
			}
			else
			{
				Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
			}
		}

		bool UpdateCurrentCell()
		{
			HexCell cell =
				grid.GetCell(mainCamera.ScreenPointToRay(Input.mousePosition));
			if (cell != currentCell)
			{
				currentCell = cell;
				return true;
			}
			return false;
		}

		void DoSelection()
		{
			grid.ClearPath();
			UpdateCurrentCell();
			if (currentCell)
			{
				selectedUnit = currentCell.Unit;
			}
		}

		void Update()
		{
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				if (Input.GetMouseButtonDown(0))
				{
					DoSelection();
				}
				else if (selectedUnit)
				{
					if (Input.GetMouseButtonDown(1))
					{
						DoMove();
					}
					else
					{
						DoPathfinding();
					}
				}
			}
		}

		void DoPathfinding()
		{
			if (UpdateCurrentCell())
			{
				if (currentCell && selectedUnit.IsValidDestination(currentCell))
				{
					grid.FindPath(selectedUnit.Location, currentCell, selectedUnit);
				}
				else
				{
					grid.ClearPath();
				}
			}
		}

		void DoMove()
		{
			if (grid.HasPath)
			{
				selectedUnit.Travel(grid.GetPath());
				grid.ClearPath();
			}
		}
	}
}
