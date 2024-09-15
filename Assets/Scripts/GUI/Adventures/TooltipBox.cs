using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GUI.Wiki
{
    public class TooltipBox : MonoBehaviour
    {
		public GenericTooltipTrigger tooltipTrigger;
		public TMP_Text text;
        public Image background;

        public void SetTooltip(string header, string content)
        {
            tooltipTrigger.header = header;
            tooltipTrigger.content = content;
            text.text = header;
        }

        public void SetColor(Color color)
        {
            var lerpedColor = Color.Lerp(color, Color.white, 0.5f);
            background.color = lerpedColor;
        }
    }
}
