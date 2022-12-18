using UnityEngine;

namespace Game.GUI
{
	public class MainGUI : MonoBehaviour
	{
		[SerializeField]
		private CanvasGroup wikiCanvas;

		public void ToggleWiki()
		{
			ToggleCanvasGroup(wikiCanvas);
		}

		private void ToggleCanvasGroup(CanvasGroup group)
		{
			if (group.alpha == 1)
			{
				group.alpha = 0;
				group.interactable = false;
				group.blocksRaycasts = false;
			}
			else
			{
				group.alpha = 1;
				group.interactable = true;
				group.blocksRaycasts = true;
			}
		}
	}
}
