using UnityEngine;

namespace Game.Visuals.Hex
{
	public struct HexHash
	{

		public float a, b, c, d, e;

		public static HexHash Create()
		{
			HexHash hash;
			hash.a = Random.value * 0.999f;
			hash.b = Random.value * 0.999f;
			hash.c = Random.value * 0.999f;
			hash.d = Random.value * 0.999f;
			hash.e = Random.value * 0.999f;
			return hash;
		}
	}

	[System.Serializable]
	public struct HexFeatureCollection
	{

		public Transform[] prefabs;

		public Transform Pick(float choice)
		{
			return prefabs[(int)(choice * prefabs.Length)];
		}
	}
}
