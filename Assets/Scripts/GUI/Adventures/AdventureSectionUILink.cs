using Game.GUI.Adventures;
using Game.Simulation;
using TMPro;
using UnityEngine;

namespace Game.GUI.Wiki
{
	public class AdventureSectionUILink : MonoBehaviour
	{
		public AdventureSection section;
		public TMP_Text text;

		public void Setup(AdventureSection section)
        {
			this.section = section;
			text.text = section.sectionTitle;
        }

		public void GoToSection()
		{
			AdventureGuide.Instance.BeginSection(section);
		}
	}
}
