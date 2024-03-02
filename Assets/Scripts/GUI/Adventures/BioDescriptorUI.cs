using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class BioDescriptorUI : SerializedMonoBehaviour
    {
        [SerializeField]
        private TMP_Text descriptorText;

        public void Fill(string value)
        {
            descriptorText.text = value.ToString();
        }

        public void SetColor(Color color)
        {
            descriptorText.color = color;
        }
    }
}
