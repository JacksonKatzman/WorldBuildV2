using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GUI.Adventures
{
    public class AdventureSectionUIComponent : SerializedMonoBehaviour
    {
        public Dictionary<Type, GameObject> prefabDictionary;
        public Transform componentRoot;

        [SerializeField]
        private AdventureSectionAdvancerButton buttonPrefab;
        [SerializeField]
        private Transform buttonRoot;

        public CanvasGroup canvasGroup;
        private List<IAdventureUIComponent> uiComponents;
        private List<AdventureSectionAdvancerButton> advancerButtons;

        //time to actually create the prefab that this hooks up to
        public void CreateSectionUI(AdventureSection section)
        {
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
                var button = Instantiate(buttonPrefab, buttonRoot);
                button.Setup(advancer);
                advancerButtons.Add(button);
            }
        }

        public void ToggleCanvasGroup(bool on)
        {
            canvasGroup.alpha = on ? 1 : 0;
            canvasGroup.interactable = on;
            canvasGroup.blocksRaycasts = on;
        }
    }
}
