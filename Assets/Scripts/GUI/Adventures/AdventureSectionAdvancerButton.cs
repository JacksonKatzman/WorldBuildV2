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

        private AdventureSectionUIComponent parentUiComponent;

        public void Setup(SectionAdvancer advancer, AdventureSectionUIComponent uIComponent)
        {
            parentUiComponent = uIComponent;
            buttonText.text = advancer.buttonText.text;
            button.onClick.AddListener(() => OnButtonPressed(advancer.nextSection));
        }

        public void SetEnabled(bool enabled)
        {
            button.enabled = enabled;
        }

        private void OnButtonPressed(AdventureSection nextSection)
        {
            AdventureGuide.Instance.BeginSection(nextSection);
            parentUiComponent.DisableOtherAdvancerButtons(this);
        }
                 
    }
}
