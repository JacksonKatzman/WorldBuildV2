using Game.Enums;
using Game.Incidents;
using Sirenix.OdinInspector;
using System.Collections.Generic;
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

        public void Append(string value)
        {
            descriptorText.text += value;
        }

        public void FillWithContextList(List<IIncidentContext> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                Append(Link(values[i]));
                if (i < values.Count - 1)
                {
                    Append("\n");
                }
            }
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
