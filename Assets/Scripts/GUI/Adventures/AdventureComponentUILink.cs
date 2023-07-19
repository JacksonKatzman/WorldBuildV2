using Game.GUI.Adventures;
using TMPro;
using UnityEngine;

namespace Game.GUI.Wiki
{
	public class AdventureComponentUILink : MonoBehaviour
	{
		public int ComponentLinkID { get; set; }
		public TMP_Text text;

		public void SnapToPath()
		{
			AdventureGuide.Instance.SetCurrentComponent(ComponentLinkID);
		}
	}
}
