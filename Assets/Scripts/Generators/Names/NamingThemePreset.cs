using Game.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Generators.Names
{
	[CreateAssetMenu(fileName = "NamingTheme", menuName = "ScriptableObjects/Names/Theme Preset", order = 1)]
	public class NamingThemePreset : ScriptableObject
	{
		[SerializeField]
		public List<NamingThemeCollectionContainer> themeCollections;

		public int averageFirstNameSyllables;
		public int averageSurnameSyllables;
		public WeightedListSerializableDictionary<string> surnameFormats;
	}
}
