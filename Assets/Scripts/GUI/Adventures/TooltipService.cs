using UnityEngine;

namespace Game.GUI.Wiki
{
	public class TooltipService : MonoBehaviour
	{
		public static TooltipService Instance { get; private set; }
		public Tooltip tooltip;

		private void Awake()
		{
			if(Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
			}
		}

		public static void ShowTooltip(string content, string header = "")
		{
			Instance.tooltip.header.enabled = !string.IsNullOrEmpty(header);
			Instance.tooltip.header.text = header;
			Instance.tooltip.content.text = content;

			Instance.tooltip.gameObject.SetActive(true);
		}

		public static void HideTooltip()
		{
			Instance.tooltip.gameObject.SetActive(false);
		}
	}
}
