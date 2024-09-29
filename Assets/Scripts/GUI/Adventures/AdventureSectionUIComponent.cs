using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GUI.Adventures
{
    public class AdventureSectionUIComponent : SerializedMonoBehaviour
    {
        public Dictionary<Type, GameObject> prefabDictionary;
        public TMP_Text sectionTitleText;
        public Transform componentRoot;

        [SerializeField]
        private AdventureSectionAdvancerButton buttonPrefab;
        [SerializeField]
        private Transform buttonRoot;

        public CanvasGroup canvasGroup;
        private List<IAdventureUIComponent> uiComponents;
        private List<AdventureSectionAdvancerButton> advancerButtons;

        public AdventureSection AdventureSection { get; private set; }

        //time to actually create the prefab that this hooks up to
        public void CreateSectionUI(AdventureSection section)
        {
            AdventureSection = section;
            sectionTitleText.text = section.sectionTitle;

            uiComponents = new List<IAdventureUIComponent>();
            foreach(var component in section.components)
            {
                if(prefabDictionary.TryGetValue(component.GetType(), out var prefab))
                {
                    var uiComponent = Instantiate(prefab, componentRoot).GetComponent<IAdventureUIComponent>();
                    uiComponent.BuildUIComponents(component);
                    uiComponents.Add(uiComponent);
                }
                else
                {
                    Debug.OutputLogger.LogWarning("Missing Adventure UI Prefab!");
                }
            }

            //build the buttons!
            advancerButtons = new List<AdventureSectionAdvancerButton>();
            foreach(var advancer in section.sectionAdvancers)
            {
                if (!advancer.isFinalSection)
                {
                    var button = Instantiate(buttonPrefab, buttonRoot);
                    button.Setup(advancer, this);
                    advancerButtons.Add(button);
                }
            }
        }

        public void DisableOtherAdvancerButtons(AdventureSectionAdvancerButton button)
        {
            foreach(var advancerButton in advancerButtons)
            {
                if(advancerButton != button)
                {
                    advancerButton.SetEnabled(false);
                }
            }
        }

        public void ToggleCanvasGroup(bool on)
        {
            //canvasGroup.alpha = on ? 1 : 0;
            //canvasGroup.interactable = on;
            //canvasGroup.blocksRaycasts = on;
            gameObject.SetActive(on);
        }
    }
}
