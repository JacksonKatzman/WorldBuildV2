using Game.GUI.Wiki;
using Game.Incidents;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GUI.Adventures
{
    public class CharacterWiki : WikiComponent<Character>
    {
        [SerializeField]
        private WikiComponent<Character> characterBio;
        [SerializeField]
        private WikiComponent<Character> characterStats;
        [SerializeField]
        private WikiComponent<Character> characterInventory;
        [SerializeField]
        private WikiComponent<Character> characterNotes;

        [SerializeField]
        private Transform tabRoot;

        private Dictionary<IWikiComponent, WikiTabSelector> components;

        override protected void Fill(Character character)
        {
            characterBio?.Fill(character);
            characterStats?.Fill(character);
            characterInventory?.Fill(character);
            characterNotes?.Fill(character);
        }

        public void SwapToTab(IWikiComponent component)
        {
            ShowComponent(component);
        }

        protected override void Preshow()
        {
            base.Preshow();
            SwapToTab(characterBio);
        }

        private void Awake()
        {
            components = new Dictionary<IWikiComponent, WikiTabSelector>();
            var fieldInfos = GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var matchingFieldInfos = fieldInfos.Where(x => x.FieldType == typeof(WikiComponent<Character>)).ToList();
            foreach(var fieldInfo in matchingFieldInfos)
            {
                var value = (WikiComponent<Character>)fieldInfo.GetValue(this);
                if(value != null)
                {
                    var tabSelector = Instantiate(WikiService.Instance.wikiTabSelectorPrefab, tabRoot);
                    tabSelector.Setup(value, fieldInfo.Name.Replace("character", ""), () => { SwapToTab(value); });
                    components.Add(value, tabSelector);
                }
            }
        }

        private void HideComponents()
        {
            foreach(var component in components)
            {
                component.Key.Hide();
            }
        }

        private void ShowComponent(IWikiComponent component)
        {
            if (component != null)
            {
                UpdateButtons(component);
                HideComponents();
                component.Show();
            }
        }

        private void UpdateButtons(IWikiComponent component)
        {
            foreach(var pair in components)
            {
                pair.Value.button.interactable = true;
            }
            if (components.TryGetValue(component, out var tab))
            {
                tab.button.interactable = false;
            }
        }
    }
}
