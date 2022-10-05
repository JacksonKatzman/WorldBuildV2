using Game.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

namespace Game.Visuals.Hex
{
	public class HexCell : MonoBehaviour
    {
		private static int MAX_ELEVATION_DIFFERENCE = 1;

		public HexCell[] neighbors = new HexCell[6];

		public HexCoordinates coordinates;
		int elevation = int.MinValue;
		int terrainTypeIndex;
		public RectTransform uiRect;

		int waterLevel;

		bool hasIncomingRiver, hasOutgoingRiver;
		HexDirection incomingRiver, outgoingRiver;

		[SerializeField]
		bool[] roads = new bool[6];

		//Likely temporary
		int urbanLevel, farmLevel, plantLevel;

		bool walled;

		int specialIndex;

		int distance;

		public HexGridChunk chunk;

		public HexCell PathFrom { get; set; }

		public void Save(BinaryWriter writer)
		{
			writer.Write((byte)terrainTypeIndex);
			writer.Write((byte)elevation);
			writer.Write((byte)waterLevel);
			writer.Write((byte)urbanLevel);
			writer.Write((byte)farmLevel);
			writer.Write((byte)plantLevel);
			writer.Write((byte)specialIndex);
			writer.Write(walled);

			//For rivers we are storing both the existance of a river and it's directions in a single byte.
			//We do this by marking the 8th bit with 1 if there is a river and 0 if not.
			if (hasIncomingRiver)
			{
				writer.Write((byte)(incomingRiver + 128));
			}
			else
			{
				writer.Write((byte)0);
			}

			if (hasOutgoingRiver)
			{
				writer.Write((byte)(outgoingRiver + 128));
			}
			else
			{
				writer.Write((byte)0);
			}

			int roadFlags = 0;
			for (int i = 0; i < roads.Length; i++)
			{
				if (roads[i])
				{
					roadFlags |= 1 << i;
				}
			}
			writer.Write((byte)roadFlags);
		}

		public void Load(BinaryReader reader)
		{
			terrainTypeIndex = reader.ReadByte();
			elevation = reader.ReadByte();
			RefreshPosition();
			waterLevel = reader.ReadByte();
			urbanLevel = reader.ReadByte();
			farmLevel = reader.ReadByte();
			plantLevel = reader.ReadByte();
			specialIndex = reader.ReadByte();
			walled = reader.ReadBoolean();

			//This means that when we go to read it, if the byte is at least 120 then it has a river.
			//To get it's direction, subtract the 128 before casting as a HexDirection
			byte riverData = reader.ReadByte();
			if (riverData >= 128)
			{
				hasIncomingRiver = true;
				incomingRiver = (HexDirection)(riverData - 128);
			}
			else
			{
				hasIncomingRiver = false;
			}

			riverData = reader.ReadByte();
			if (riverData >= 128)
			{
				hasOutgoingRiver = true;
				outgoingRiver = (HexDirection)(riverData - 128);
			}
			else
			{
				hasOutgoingRiver = false;
			}

			int roadFlags = reader.ReadByte();
			for (int i = 0; i < roads.Length; i++)
			{
				roads[i] = (roadFlags & (1 << i)) != 0;
			}
		}

		void RefreshPosition()
		{
			Vector3 position = transform.localPosition;
			position.y = elevation * HexMetrics.elevationStep;
			position.y +=
				(HexMetrics.SampleNoise(position).y * 2f - 1f) *
				HexMetrics.elevationPerturbStrength;
			transform.localPosition = position;

			Vector3 uiPosition = uiRect.localPosition;
			uiPosition.z = -position.y;
			uiRect.localPosition = uiPosition;
		}

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
				RefreshPosition();

				ValidateRivers();

				for (int i = 0; i < roads.Length; i++)
				{
					if (roads[i] && GetElevationDifference((HexDirection)i) > 1)
					{
						SetRoad(i, false);
					}
				}

				Refresh();
			}
		}

		public int TerrainTypeIndex
		{
			get
			{
				return terrainTypeIndex;
			}
			set
			{
				if (terrainTypeIndex != value)
				{
					terrainTypeIndex = value;
					Refresh();
				}
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
		public HexDirection RiverBeginOrEndDirection
		{
			get
			{
				return hasIncomingRiver ? incomingRiver : outgoingRiver;
			}
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
					(elevation + HexMetrics.waterElevationOffset) *
					HexMetrics.elevationStep;
			}
		}

		public float WaterSurfaceY
		{
			get
			{
				return
					(waterLevel + HexMetrics.waterElevationOffset) *
					HexMetrics.elevationStep;
			}
		}

		public bool HasRoads
		{
			get
			{
				for (int i = 0; i < roads.Length; i++)
				{
					if (roads[i])
					{
						return true;
					}
				}
				return false;
			}
		}

		public int WaterLevel
		{
			get
			{
				return waterLevel;
			}
			set
			{
				if (waterLevel == value)
				{
					return;
				}
				waterLevel = value;
				ValidateRivers();
				Refresh();
			}
		}

		public int UrbanLevel
		{
			get
			{
				return urbanLevel;
			}
			set
			{
				if (urbanLevel != value)
				{
					urbanLevel = value;
					RefreshSelfOnly();
				}
			}
		}

		public int FarmLevel
		{
			get
			{
				return farmLevel;
			}
			set
			{
				if (farmLevel != value)
				{
					farmLevel = value;
					RefreshSelfOnly();
				}
			}
		}

		public int PlantLevel
		{
			get
			{
				return plantLevel;
			}
			set
			{
				if (plantLevel != value)
				{
					plantLevel = value;
					RefreshSelfOnly();
				}
			}
		}

		public bool IsUnderwater
		{
			get
			{
				return waterLevel > elevation;
			}
		}

		public bool Walled
		{
			get
			{
				return walled;
			}
			set
			{
				if (walled != value)
				{
					walled = value;
					Refresh();
				}
			}
		}

		public int SpecialIndex
		{
			get
			{
				return specialIndex;
			}
			set
			{
				if (specialIndex != value &&!HasRiver)
				{
					specialIndex = value;
					RemoveRoads();
					RefreshSelfOnly();
				}
			}
		}

		public bool IsSpecial
		{
			get
			{
				return specialIndex > 0;
			}
		}
		public int Distance
		{
			get
			{
				return distance;
			}
			set
			{
				distance = value;
				UpdateDistanceLabel();
			}
		}

		public int SearchHeuristic { get; set; }
		public HexCell NextWithSamePriority { get; set; }

		public int SearchPriority
		{
			get
			{
				return distance + SearchHeuristic;
			}
		}

		bool IsValidRiverDestination(HexCell neighbor)
		{
			return neighbor && (
				elevation >= neighbor.elevation || waterLevel == neighbor.elevation
			);
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
			if(!IsValidRiverDestination(neighbor)) {
				return;
			}

			RemoveOutgoingRiver();
			if (hasIncomingRiver && incomingRiver == direction)
			{
				RemoveIncomingRiver();
			}

			hasOutgoingRiver = true;
			outgoingRiver = direction;
			specialIndex = 0;
			//RefreshSelfOnly();

			neighbor.RemoveIncomingRiver();
			neighbor.hasIncomingRiver = true;
			neighbor.incomingRiver = direction.Opposite();
			neighbor.specialIndex = 0;
			//neighbor.RefreshSelfOnly();

			SetRoad((int)direction, false);
		}

		void ValidateRivers()
		{
			if (
				hasOutgoingRiver &&
				!IsValidRiverDestination(GetNeighbor(outgoingRiver))
			)
			{
				RemoveOutgoingRiver();
			}
			if (
				hasIncomingRiver &&
				!GetNeighbor(incomingRiver).IsValidRiverDestination(this)
			)
			{
				RemoveIncomingRiver();
			}
		}

		public bool HasRoadThroughEdge(HexDirection direction)
		{
			return roads[(int)direction];
		}
		public void AddRoad(HexDirection direction)
		{
			if (!roads[(int)direction] && !HasRiverThroughEdge(direction) &&
				!IsSpecial && !GetNeighbor(direction).IsSpecial &&
				GetElevationDifference(direction) <= MAX_ELEVATION_DIFFERENCE)
			{
				SetRoad((int)direction, true);
			}
		}

		public void RemoveRoads()
		{
			for (int i = 0; i < neighbors.Length; i++)
			{
				if (roads[i])
				{
					SetRoad(i, false);
				}
			}
		}

		void SetRoad(int index, bool state)
		{
			roads[index] = state;
			neighbors[index].roads[(int)((HexDirection)index).Opposite()] = state;
			neighbors[index].RefreshSelfOnly();
			RefreshSelfOnly();
		}

		public int GetElevationDifference(HexDirection direction)
		{
			int difference = elevation - GetNeighbor(direction).elevation;
			return difference >= 0 ? difference : -difference;
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

		void UpdateDistanceLabel()
		{
			TMP_Text label = uiRect.GetComponent<TMP_Text>();
			label.text = distance == int.MaxValue ? "" : distance.ToString();
		}

		public void DisableHighlight()
		{
			Image highlight = uiRect.GetChild(0).GetComponent<Image>();
			highlight.enabled = false;
		}

		public void EnableHighlight(Color color)
		{
			Image highlight = uiRect.GetChild(0).GetComponent<Image>();
			highlight.color = color;
			highlight.enabled = true;
		}
	}
}
