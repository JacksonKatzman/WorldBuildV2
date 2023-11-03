using Game.Generators.Names;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Incidents
{
	public class AssetService : SerializedMonoBehaviour
	{
		public static AssetService Instance { get; private set; }
		public TextCollection incidents;
		public SerializedObjectCollectionContainer objectData;
		public NamingThemePreset monsterPreset;

		public NamingTheme MonsterTheme { get; set; }
		public void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
				MonsterTheme = new NamingTheme(monsterPreset);
			}
		}
	}
}