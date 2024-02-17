using Game.Incidents;
using UnityEngine;

namespace Game.Terrain
{
    public class HexGridOverlayChunk : HexGridChunk
    {
		public override void AddCell(int index, HexCell cell)
		{
			cells[index] = cell;
		}

		public void HandleCollectionType(HexCollection collection, int id)
		{
			if (collection.CollectionType == HexCollection.HexCollectionType.RIVER)
			{
				terrain.GetComponent<MeshRenderer>().enabled = false;
				roads.GetComponent<MeshRenderer>().enabled = false;
				water.GetComponent<MeshRenderer>().enabled = false;
				waterShore.GetComponent<MeshRenderer>().enabled = false;
				estuaries.GetComponent<MeshRenderer>().enabled = false;

				//raise by a very small amount so that theres no z-fighting when trying to highlight
				transform.LeanSetPosY(0.001f);
				name = $"Hex Collecton Chunk {id} RIVER";
			}
			else
			{
				rivers.GetComponent<MeshRenderer>().enabled = false;
				name = $"Hex Collecton Chunk {id}";
			}

			if (collection.CollectionType == HexCollection.HexCollectionType.LAKE)
			{
				//raise by a very small amount so that theres no z-fighting when trying to highlight
				//has to be higher than rivers
				transform.LeanSetPosY(0.0011f);
				name = $"Hex Collecton Chunk {id} LAKE";
			}
		}

		public void InitializeTerrainHighlighting(HexCollection collection)
		{
			if (collection.CollectionType == HexCollection.HexCollectionType.RIVER)
			{
				rivers.gameObject.GetComponent<HexChunkHighlight>().collection = collection;
			}
			else
			{
				terrain.gameObject.GetComponent<HexChunkHighlight>().collection = collection;
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
		}

        protected override void AddBridge(Vector3 roadCenter1, Vector3 roadCenter2)
        {
            
        }
    }
}