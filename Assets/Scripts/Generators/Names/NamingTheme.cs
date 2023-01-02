using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Generators.Names
{
	[CreateAssetMenu(fileName = "NamingTheme", menuName = "ScriptableObjects/Names/Theme Preset", order = 1)]
	public class NamingThemePreset : ScriptableObject
	{
		[SerializeField]
		public List<NamingThemeCollection> themeCollections;
	}

	public class NamingTheme
	{
		private NamingThemeCollection collection;
		public NamingTheme(NamingThemePreset preset)
		{
			collection = new NamingThemeCollection(preset.themeCollections[0]);

			for(int i = 1; i < preset.themeCollections.Count; i++)
			{
				collection = collection + preset.themeCollections[i];
			}
		}
	}
}
