using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Simulation
{
    [HideReferenceObjectPicker]
    public class AdventureSection : IAdventureComponent
    {
        public string sectionTitle;
        [ListDrawerSettings(Expanded = true)]
        public List<IAdventureComponent> components = new List<IAdventureComponent>();
        [ListDrawerSettings(CustomAddFunction = "AddSectionAdvancer", Expanded = true)]
        public List<SectionAdvancer> sectionAdvancers = new List<SectionAdvancer>() { new SectionAdvancer() };
        public bool Completed { get; set; }

        public AdventureSection() { }
        public AdventureSection(string title, AdventureSection next)
        {
            sectionTitle = title;
            sectionAdvancers.First().nextSection = next;
        }

        public void UpdateRetrieverIds(int oldID, int newID)
        {
            foreach (var component in components)
            {
                component.UpdateRetrieverIds(oldID, newID);
            }

            foreach(var sectionAdvancer in sectionAdvancers)
            {
                sectionAdvancer.buttonText.UpdateIDs(oldID, newID);
            }
        }

        private void AddSectionAdvancer()
        {
            sectionAdvancers.Add(new SectionAdvancer());
        }
    }

    [HideReferenceObjectPicker]
    public class SectionAdvancer
    {
        public bool isFinalSection;
        [HideIf("@this.isFinalSection")]
        public AdventureComponentTextField buttonText = new AdventureComponentTextField("Next");
        [ColorPalette, HideIf("@this.isFinalSection")]
        public Color buttonColor;
        [HideIf("@this.isFinalSection"), ValueDropdown("GetSectionTitles"), OnValueChanged("SetSectionByTitle")]
        public string nextSectionKey;
        [HideIf("@this.isFinalSection"), ShowInInspector]
        public bool SectionSelected => nextSection != null;
        [HideInInspector]
        public AdventureSection nextSection;

        private IEnumerable<string> GetSectionTitles()
        {
            return AdventureEncounterObject.Current?.sections?.Select(x => x.sectionTitle);
        }

        private void SetSectionByTitle()
        {
            nextSection = AdventureEncounterObject.Current.sections.First(x => x.sectionTitle == nextSectionKey);
        }
    }
}
