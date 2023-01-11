using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Generators.Names
{
	[CreateAssetMenu(fileName = "ThemeCollection", menuName = "ScriptableObjects/Names/Theme Collection", order = 2)]
	public class NamingThemeCollectionContainer : SerializedScriptableObject
	{
		public NamingThemeCollection collection;
		public Dictionary<int, List<string>> test;

		private void Awake()
		{
			OutputLogger.LogWarning(">*> Container Awake (Resets collection by newing)");
			collection = new NamingThemeCollection();
		}
	}
}
