using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class StatContainerUI : SerializedMonoBehaviour
    {
        [SerializeField]
        private TMP_Text statText;
        [SerializeField]
        private TMP_Text modifierText;

        public void Fill(int value)
        {
            statText.text = value.ToString();
            var modifier = (value - 10) / 2;
            modifierText.text = modifier >= 0 ? $"+{modifier}" : modifier.ToString();
        }

        public void FillUnknown()
        {
            statText.text = "?";
            modifierText.text = string.Empty;
        }
    }
}
