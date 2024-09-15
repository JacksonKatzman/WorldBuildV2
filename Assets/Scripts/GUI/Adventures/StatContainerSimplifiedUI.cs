using TMPro;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class StatContainerSimplifiedUI : WikiComponent<int>
    {
        [SerializeField]
        private TMP_Text statText;

        override protected void Fill(int value)
        {
            statText.text = value.ToString();
            var modifier = (value - 10) / 2;
            var modifierText = modifier >= 0 ? $"+{modifier}" : modifier.ToString();
            statText.text += $" ({modifierText})";
        }

        public virtual void FillUnknown()
        {
            statText.text = "?";
        }

        public override void Clear()
        {
            FillUnknown();
        }
    }
}
