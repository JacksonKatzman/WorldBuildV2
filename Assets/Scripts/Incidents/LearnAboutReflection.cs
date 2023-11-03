using Game.Debug;
using Game.Generators.Names;
using UnityEngine;

namespace Game.Incidents
{
	public class LearnAboutReflection : MonoBehaviour
	{
		[SerializeField]
		public NamingThemePreset preset;

		private NamingTheme theme;

		public void TestName()
		{
			if(theme == null)
			{
				theme = new NamingTheme(preset);
			}

			//var name = theme.GenerateName(Enums.Gender.MALE);

			OutputLogger.Log("Generated Name is: " + name);
		}
	}
}