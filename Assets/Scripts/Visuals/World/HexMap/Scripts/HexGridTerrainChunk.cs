using UnityEngine;

namespace Game.Terrain
{
    public class HexGridTerrainChunk : HexGridChunk
    {
		public HexFeatureManager features;
		public override void AddCell(int index, HexCell cell)
		{
			cells[index] = cell;
			cell.chunk = this;
			cell.transform.SetParent(transform, false);
			cell.hexCellLabel.rectTransform.SetParent(gridCanvas.transform, false);
		}

		public void AddFeatures()
		{
			for (int i = 0; i < cells.Length; i++)
			{
				var cell = cells[i];
				//cell.hexCellLabel.hexCellText.text = cell.Index.ToString();

				if (!cell.IsUnderwater)
				{
					features.AddHexFeature(cell, cell.Position);
				}
			}
		}

		public override void Triangulate()
		{
			terrain.Clear();
			rivers.Clear();
			roads.Clear();
			water.Clear();
			waterShore.Clear();
			estuaries.Clear();
			features.Clear();

			for (int i = 0; i < cells.Length; i++)
			{
				Triangulate(cells[i]);
			}

			terrain.Apply();
			rivers.Apply();
			roads.Apply();
			water.Apply();
			waterShore.Apply();
			estuaries.Apply();
			features.Apply();
		}

        protected override void AddBridge(Vector3 roadCenter1, Vector3 roadCenter2)
        {
			features.AddBridge(roadCenter1, roadCenter2);
        }
    }
}