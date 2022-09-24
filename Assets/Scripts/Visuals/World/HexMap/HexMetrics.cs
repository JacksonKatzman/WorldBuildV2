using Game.Enums;
using UnityEngine;

namespace Game.Visuals.Hex
{
	public static class HexMetrics
	{
		public static float outerRadius = 10f;
		public static float innerRadius = outerRadius * 0.866025404f;
		public static float offset = outerRadius;
		public static Vector3[] corners = {
			new Vector3(0f, 0f, outerRadius),
			new Vector3(innerRadius, 0f, 0.5f * outerRadius),
			new Vector3(innerRadius, 0f, -0.5f * outerRadius),
			new Vector3(0f, 0f, -outerRadius),
			new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
			new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
			new Vector3(0f, 0f, outerRadius)
		};

		public const float solidFactor = 0.75f;

		public const float blendFactor = 1f - solidFactor;

		public static Vector3 GetFirstCorner(HexDirection direction)
		{
			return corners[(int)direction];
		}

		public static Vector3 GetSecondCorner(HexDirection direction)
		{
			return corners[(int)direction + 1];
		}

		public static Vector3 GetFirstSolidCorner(HexDirection direction)
		{
			return corners[(int)direction] * solidFactor;
		}

		public static Vector3 GetSecondSolidCorner(HexDirection direction)
		{
			return corners[(int)direction + 1] * solidFactor;
		}

		public static Vector3 GetBridge(HexDirection direction)
		{
			return (corners[(int)direction] + corners[(int)direction + 1]) *
				blendFactor;
		}
	}
}
