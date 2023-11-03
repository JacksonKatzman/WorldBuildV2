using Game.Data;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Generators.Names
{
	[CreateAssetMenu(fileName = "NamingTheme", menuName = "ScriptableObjects/Names/Theme Preset", order = 1)]
	public class NamingThemePreset : AbstractNamingThemeObject
	{
		public TextAsset maleNames;
		public TextAsset femaleNames;
		public TextAsset androgynousNames;
		public TextAsset standardSurnames;
		public TextAsset frontPartialNames;
		public TextAsset backPartialNames;
		public List<string> qualifiers;

		public TextAsset factionNames;

		public List<string> maleNameFormats;
		public List<string> femaleNameFormats;
		public List<string> compositeSurnameFormats;
		public List<string> townNameFormats;
		public List<string> factionNameFormats;

		[SerializeField]
		public List<NamingThemeCollection> themeCollections;

		/*
		[SerializeField, ValueDropdown("GetListOptions", IsUniqueList = true)]
		private string listToAddTo;

		[TextArea]
		public string input;
		*/

		public NamingThemePreset()
		{
			GetListCollection();
		}
	}
}
