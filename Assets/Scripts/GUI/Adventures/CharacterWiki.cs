using Game.Incidents;
using System.Collections.Generic;
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
        private Button characterBioButton;
        [SerializeField]
        private Button characterStatsButton;
        [SerializeField]
        private Button characterInventoryButton;
        [SerializeField]
        private Button characterNotesButton;

        private List<IWikiComponent> components;

        override protected void Fill(Character character)
        {
            characterBio?.Fill(character);
            characterStats?.Fill(character);
            characterInventory?.Fill(character);
            characterNotes?.Fill(character);
        }

        public void SwapToBio()
        {
            ShowComponent(characterBio);
        }

        public void SwapToStats()
        {
            ShowComponent(characterStats);
        }

        public void SwapToInventory()
        {
            ShowComponent(characterInventory);
        }

        public void SwapToNotes()
        {
            ShowComponent(characterNotes);
        }

        private void Awake()
        {
            components = new List<IWikiComponent>();
            components.Add(characterBio);
            components.Add(characterStats);
            components.Add(characterInventory);
            components.Add(characterNotes);
        }

        private void HideComponents()
        {
            foreach(var component in components)
            {
                component.Hide();
            }
        }

        private void ShowComponent(IWikiComponent component)
        {
            HideComponents();
            component.Show();
        }
    }
}
