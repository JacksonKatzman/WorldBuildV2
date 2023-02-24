using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.GUI.Wiki
{
	public class GenericTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		private static LTDescr delay;
		public string header;
		[TextArea(15, 20)]
		public string content;
		public void OnPointerEnter(PointerEventData eventData)
		{
			delay = LeanTween.delayedCall(0.75f, () =>
			{
				TooltipService.ShowTooltip(content, header);
			});
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			LeanTween.cancel(delay.uniqueId);
			TooltipService.HideTooltip();
		}
	}
}
