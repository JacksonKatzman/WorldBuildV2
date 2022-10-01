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
		int elevation = int.MinValue;
		public Color color;
		public RectTransform uiRect;

		bool hasIncomingRiver, hasOutgoingRiver;
		HexDirection incomingRiver, outgoingRiver;

		public HexGridChunk chunk;
		public int Elevation
		{
			get
			{
				return elevation;
			}
			set
			{
				if(elevation == value)
				{
					return;
				}

				elevation = value;
				Vector3 position = transform.localPosition;
				position.y = value * HexMetrics.elevationStep;
				position.y +=
				(HexMetrics.SampleNoise(position).y * 2f - 1f) *
				HexMetrics.elevationPerturbStrength;
				transform.localPosition = position;

				Vector3 uiPosition = uiRect.localPosition;
				uiPosition.z = -position.y;
				uiRect.localPosition = uiPosition;

				if (
					hasOutgoingRiver &&
					elevation < GetNeighbor(outgoingRiver).elevation)
				{
					RemoveOutgoingRiver();
				}
				if (
					hasIncomingRiver &&
					elevation > GetNeighbor(incomingRiver).elevation)
				{
					RemoveIncomingRiver();
				}

				Refresh();
			}
		}

		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				if (color == value)
				{
					return;
				}
				color = value;
				Refresh();
			}
		}

		public Vector3 Position
		{
			get
			{
				return transform.localPosition;
			}
		}

		public bool HasIncomingRiver
		{
			get
			{
				return hasIncomingRiver;
			}
		}

		public bool HasOutgoingRiver
		{
			get
			{
				return hasOutgoingRiver;
			}
		}

		public HexDirection IncomingRiver
		{
			get
			{
				return incomingRiver;
			}
		}

		public HexDirection OutgoingRiver
		{
			get
			{
				return outgoingRiver;
			}
		}

		public bool HasRiver
		{
			get
			{
				return hasIncomingRiver || hasOutgoingRiver;
			}
		}

		public HexCell GetNeighbor(HexDirection direction)
		{
			return neighbors[(int)direction];
		}

		public bool HasRiverBeginOrEnd
		{
			get
			{
				return hasIncomingRiver != hasOutgoingRiver;
			}
		}

		public bool HasRiverThroughEdge(HexDirection direction)
		{
			return
				hasIncomingRiver && incomingRiver == direction ||
				hasOutgoingRiver && outgoingRiver == direction;
		}
		public float StreamBedY
		{
			get
			{
				return
					(elevation + HexMetrics.streamBedElevationOffset) *
					HexMetrics.elevationStep;
			}
		}

		public float RiverSurfaceY
		{
			get
			{
				return
					(elevation + HexMetrics.riverSurfaceElevationOffset) *
					HexMetrics.elevationStep;
			}
		}

		public void RemoveOutgoingRiver()
		{
			if (!hasOutgoingRiver)
			{
				return;
			}
			hasOutgoingRiver = false;
			RefreshSelfOnly();

			HexCell neighbor = GetNeighbor(outgoingRiver);
			neighbor.hasIncomingRiver = false;
			neighbor.RefreshSelfOnly();
		}

		public void RemoveIncomingRiver()
		{
			if (!hasIncomingRiver)
			{
				return;
			}
			hasIncomingRiver = false;
			RefreshSelfOnly();

			HexCell neighbor = GetNeighbor(incomingRiver);
			neighbor.hasOutgoingRiver = false;
			neighbor.RefreshSelfOnly();
		}

		public void RemoveRiver()
		{
			RemoveOutgoingRiver();
			RemoveIncomingRiver();
		}

		public void SetOutgoingRiver(HexDirection direction)
		{
			if (hasOutgoingRiver && outgoingRiver == direction)
			{
				return;
			}

			HexCell neighbor = GetNeighbor(direction);
			if (!neighbor || elevation < neighbor.elevation)
			{
				return;
			}

			RemoveOutgoingRiver();
			if (hasIncomingRiver && incomingRiver == direction)
			{
				RemoveIncomingRiver();
			}

			hasOutgoingRiver = true;
			outgoingRiver = direction;
			RefreshSelfOnly();

			neighbor.RemoveIncomingRiver();
			neighbor.hasIncomingRiver = true;
			neighbor.incomingRiver = direction.Opposite();
			neighbor.RefreshSelfOnly();
		}

		public void SetNeighbor(HexDirection direction, HexCell cell)
		{
			neighbors[(int)direction] = cell;
			cell.neighbors[(int)direction.Opposite()] = this;
		}

		public HexEdgeType GetEdgeType(HexDirection direction)
		{
			return HexMetrics.GetEdgeType(
				elevation, neighbors[(int)direction].elevation
			);
		}

		public HexEdgeType GetEdgeType(HexCell otherCell)
		{
			return HexMetrics.GetEdgeType(
				elevation, otherCell.elevation
			);
		}

		void Refresh()
		{
			if (chunk)
			{
				chunk.Refresh();
				for (int i = 0; i < neighbors.Length; i++)
				{
					HexCell neighbor = neighbors[i];
					if (neighbor != null && neighbor.chunk != chunk)
					{
						neighbor.chunk.Refresh();
					}
				}
			}
		}

		void RefreshSelfOnly()
		{
			chunk.Refresh();
		}
	}
}
