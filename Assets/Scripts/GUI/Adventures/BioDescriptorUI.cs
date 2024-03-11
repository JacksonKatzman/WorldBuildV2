using Game.Enums;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class BioDescriptorUI : WikiComponent<string>
    {
        [SerializeField]
        private TMP_Text descriptorText;

        override protected void Fill(string value)
        {
            descriptorText.text = value.ToString();
        }

        public override void Clear()
        {
            descriptorText.text = string.Empty;
        }

        public void SetColor(Color color)
        {
            var hex = ColorUtility.ToHtmlStringRGB(color);
            descriptorText.text = $"<color=#{hex}>{descriptorText.text}</color>";
        }

        public override void UpdateByFamiliarity(ContextFamiliarity familiarity)
        {
            descriptorText.gameObject.SetActive(familiarity >= FamiliarityRequirement);
        }
    }
}
