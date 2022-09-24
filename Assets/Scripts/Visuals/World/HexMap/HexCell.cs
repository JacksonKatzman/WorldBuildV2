using Game.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Visuals.Hex
{
	public class HexCell : MonoBehaviour
    {
		public HexCell[] neighbors = new HexCell[6];

		public HexCoordinates coordinates;
		public Color color;

		public HexCell GetNeighbor(HexDirection direction)
		{
			return neighbors[(int)direction];
		}

		public void SetNeighbor(HexDirection direction, HexCell cell)
		{
			neighbors[(int)direction] = cell;
			cell.neighbors[(int)direction.Opposite()] = this;
		}
	}
}
