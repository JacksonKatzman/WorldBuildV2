using Game.Generators.Names;
using Game.Incidents;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Simulation
{
	[CreateAssetMenu(fileName = "RacePreset", menuName = "ScriptableObjects/Simulation/Race Preset", order = 1)]
	public class RacePreset : SerializedScriptableObject
	{
		public int minAge;
		public int maxAge;

		public List<OrganizationTemplate> organizationTemplates;

		[SerializeField]
		public NamingThemePreset namingTheme;

		public List<GameObject> flatCityPresets;
		public List<GameObject> hilledCityPresets;
		public List<GameObject> mountainCityPresets;

		public List<GameObject> flatWalls;
		public List<GameObject> gateWalls;
		public List<GameObject> riverWalls;
		public List<GameObject> outerTurrets;
	}
}
