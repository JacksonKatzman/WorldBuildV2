using Game.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Visuals.Hex
{
	public class HexGrid : MonoBehaviour
	{
		[SerializeField]
		HexMesh hexMesh;

		[SerializeField]
		private Canvas gridCanvas;

		[SerializeField]
		public GameObject cellLabelPrefab;

		public Color defaultColor = Color.white;

		public int width = 6;
		public int height = 6;

		public HexCell cellPrefab;

		HexCell[] cells;

		void Awake()
		{
			cells = new HexCell[height * width];

			for (int z = 0, i = 0; z < height; z++)
			{
				for (int x = 0; x < width; x++)
				{
					CreateCell(x, z, i++);
				}
			}

			hexMesh.Init();
			hexMesh.Triangulate(cells);
		}

		void CreateCell(int x, int z, int i)
		{
			Vector3 position;
			position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
			position.y = 0f;
			position.z = z * (HexMetrics.outerRadius * 1.5f);

			HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
			cell.transform.SetParent(transform, false);
			cell.transform.localPosition = position;
			cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
			cell.color = defaultColor;

			if (x > 0)
			{
				cell.SetNeighbor(HexDirection.W, cells[i - 1]);
			}
			if (z > 0)
			{
				if ((z & 1) == 0)
				{
					cell.SetNeighbor(HexDirection.SE, cells[i - width]);
					if (x > 0)
					{
						cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
					}
				}
				else
				{
					cell.SetNeighbor(HexDirection.SW, cells[i - width]);
					if (x < width - 1)
					{
						cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
					}
				}
			}

			var labelObject = Instantiate(cellLabelPrefab);
			TMP_Text label = labelObject.GetComponent<TMP_Text>();
			label.rectTransform.SetParent(gridCanvas.transform, false);
			label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
			label.text = cell.coordinates.ToStringOnSeparateLines();
			cell.uiRect = label.rectTransform;
		}

		public HexCell GetCell(Vector3 position)
		{
			position = transform.InverseTransformPoint(position);
			HexCoordinates coordinates = HexCoordinates.FromPosition(position);
			int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
			return cells[index];
		}

		public void Refresh()
		{
			hexMesh.Triangulate(cells);
		}
	}
}
