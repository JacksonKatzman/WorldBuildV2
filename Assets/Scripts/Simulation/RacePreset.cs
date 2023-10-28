using Game.Generators.Names;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Simulation
{
	[CreateAssetMenu(fileName = "RacePreset", menuName = "ScriptableObjects/Simulation/Race Preset", order = 1)]
	public class RacePreset : SerializedScriptableObject
	{
		public int minAge;
		public int maxAge;

		public AnimationCurve ageCurve;

		[SerializeField]
		public NamingThemePreset namingTheme;
	}
}
