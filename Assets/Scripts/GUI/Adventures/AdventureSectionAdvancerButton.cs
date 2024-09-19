using Game.Simulation;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GUI.Adventures
{
    public class AdventureSectionAdvancerButton : SerializedMonoBehaviour
    {
        [SerializeField]
        private TMP_Text buttonText;
        [SerializeField]
        private Button button;

        public void Setup(SectionAdvancer advancer)
        {
            buttonText.text = advancer.buttonText.text;
            button.onClick.AddListener(() => AdventureGuide.Instance.BeginSection(advancer.nextSection));
        }
    }
}
