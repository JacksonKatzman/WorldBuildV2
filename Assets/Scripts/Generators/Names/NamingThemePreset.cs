using Game.Data;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Generators.Names
{
	[CreateAssetMenu(fileName = "NamingTheme", menuName = "ScriptableObjects/Names/Theme Preset", order = 1)]
	public class NamingThemePreset : SerializedScriptableObject
	{
		[SerializeField]
		public List<NamingThemeCollection> themeCollections;

		public int minFirstNameSyllables;
		public int maxFirstNameSyllables;
		public int minSurnameSyllables;
		public int maxSurnameSyllables;
		public Dictionary<int, List<string>> personNameFormats;
		public Dictionary<int, List<string>> townNameFormats;
		public Dictionary<int, List<string>> factionNameFormats;
	}
}
