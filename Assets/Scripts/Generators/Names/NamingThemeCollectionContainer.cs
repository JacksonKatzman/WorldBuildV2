using System;
using UnityEngine;

namespace Game.Generators.Names
{
	[Serializable]
	[CreateAssetMenu(fileName = "ThemeCollection", menuName = "ScriptableObjects/Names/Theme Collection", order = 2)]
	public class NamingThemeCollectionContainer : ScriptableObject
	{
		public NamingThemeCollection collection;

		private void Awake()
		{
			collection = new NamingThemeCollection();
		}
	}
}
