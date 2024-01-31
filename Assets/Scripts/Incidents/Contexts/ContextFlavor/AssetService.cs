using Game.Generators.Names;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Game.Enums;
using Game.Terrain;

namespace Game.Incidents
{
	public class AssetService : SerializedMonoBehaviour
	{
		public static AssetService Instance { get; private set; }
		public TextCollection incidents;
		public SerializedObjectCollectionContainer objectData;
		public Dictionary<CreatureType, NamingThemePreset> monsterPresets;
		public Transform testMountain;

		[SerializeField]
		private BiomeDataContainer biomeDataContainer;
		public BiomeDataContainer BiomeDataContainer => biomeDataContainer;

		public Dictionary<CreatureType, NamingTheme> MonsterThemes { get; set; }
		public void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
				MonsterThemes = new Dictionary<CreatureType, NamingTheme>();
				foreach(var pair in monsterPresets)
				{
					MonsterThemes.Add(pair.Key, new NamingTheme(pair.Value));
				}
			}
		}
	}
}