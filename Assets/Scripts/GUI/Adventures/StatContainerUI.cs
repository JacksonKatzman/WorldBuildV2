using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class StatContainerUI : WikiComponent<int>
    {
        [SerializeField]
        private TMP_Text statText;
        [SerializeField]
        private TMP_Text modifierText;

        override protected void Fill(int value)
        {
            statText.text = value.ToString();
            var modifier = (value - 10) / 2;
            modifierText.text = modifier >= 0 ? $"+{modifier}" : modifier.ToString();
        }

        public virtual void FillUnknown()
        {
            statText.text = "?";
            modifierText.text = string.Empty;
        }

        public override void Clear()
        {
            FillUnknown();
        }
    }
}
