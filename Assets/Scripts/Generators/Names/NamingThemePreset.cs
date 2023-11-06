using Game.Data;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Generators.Names
{
	[CreateAssetMenu(fileName = "NamingTheme", menuName = "ScriptableObjects/Names/Theme Preset", order = 1)]
	public class NamingThemePreset : AbstractNamingThemeObject
	{
		public List<string> maleNames;
		public List<string> malePrefixes;
		public List<string> maleSuffixes;

		public List<string> femaleNames;
		public List<string> femalePrefixes;
		public List<string> femaleSuffixes;

		public List<string> androgynousNames;
		public List<string> androgynousPrefixes;
		public List<string> androgynousSuffixes;

		public List<string> surnames;
		public List<string> surnamePrefixes;
		public List<string> surnameSuffixes;

		public List<string> qualifiers;

		public List<string> factionNames;
		public List<string> factionPrefixes;
		public List<string> factionSuffixes;

		public List<string> cityNames;
		public List<string> cityPrefixes;
		public List<string> citySuffixes;

		public List<string> townNames;
		public List<string> townPrefixes;
		public List<string> townSuffixes;

		public List<string> maleNameFormats;
		public List<string> femaleNameFormats;
		public List<string> compositeSurnameFormats;
		public List<string> cityNameFormats;
		public List<string> townNameFormats;
		public List<string> factionNameFormats;

		[SerializeField]
		public List<NamingThemeCollection> themeCollections;

		public NamingThemePreset()
		{
			GetListCollection();
		}
	}
}
