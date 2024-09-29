﻿using Game.Simulation;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            buttonText.text = advancer.buttonText.Text;
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