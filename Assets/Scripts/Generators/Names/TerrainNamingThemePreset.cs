using Game.Terrain;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Generators.Names
{
	[CreateAssetMenu(fileName = "LocationNamingThemePreset", menuName = "ScriptableObjects/Names/Location Theme Preset", order = 6)]
	public class TerrainNamingThemePreset : SerializedScriptableObject
	{
		public List<string> mountainFormats;
		public List<string> lakeFormats;
		public List<string> islandFormats;
		public Dictionary<BiomeTerrainType, List<string>> biomeFormats;
	}
}
