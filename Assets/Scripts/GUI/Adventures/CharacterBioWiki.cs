using Game.Enums;
using Game.Incidents;
using Game.Simulation;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class CharacterBioWiki : WikiComponent<Character>
    {
        [SerializeField]
        private TMP_Text characterNameText;

        [SerializeField]
        private BioDescriptorUI ageDescriptor;
        [SerializeField]
        private BioDescriptorUI statusDescriptor;
        [SerializeField]
        private BioDescriptorUI genderDescriptor;
        [SerializeField]
        private BioDescriptorUI raceDescriptor;
        [SerializeField]
        private BioDescriptorUI factionDescriptor;
        [SerializeField]
        private BioDescriptorUI metDescriptor;
        [SerializeField]
        private BioDescriptorUI lastSeenDescriptor;
        [SerializeField]
        private BioDescriptorUI eyesDescriptor;
        [SerializeField]
        private BioDescriptorUI hairDescriptor;
        [SerializeField]
        private BioDescriptorUI heightDescriptor;
        [SerializeField]
        private BioDescriptorUI buildDescriptor;
        [SerializeField]
        private BioDescriptorUI notablesDescriptor;
        [SerializeField]
        private BioDescriptorUI accentDescriptor;
        [SerializeField]
        private BioDescriptorUI dialectDescriptor;
        [SerializeField]
        private BioDescriptorUI personalityDescriptor;
        [SerializeField]
        private BioDescriptorUI idealsDescriptor;
        [SerializeField]
        private BioDescriptorUI bondsDescriptor;
        [SerializeField]
        private BioDescriptorUI flawsDescriptor;

        private List<IWikiComponent> componentList;

        override public void UpdateByFamiliarity(ContextFamiliarity familiarity)
        {
            foreach(var component in componentList)
            {
                component.UpdateByFamiliarity(familiarity);
            }
        }

        override protected void Fill(Character character)
        {
            characterNameText.text = character.CharacterName.GetFullName();
            ageDescriptor.Fill(character.Age.ToString());
            genderDescriptor.Fill(character.Gender.ToString());
            raceDescriptor.Fill(Link(character.AffiliatedRace));
            factionDescriptor.Fill(Link(character.AffiliatedFaction));
            //will do the rest once I have a way to generate them

            var alive = ContextDictionaryProvider.CurrentContexts[typeof(Character)].Contains(character);
            statusDescriptor.Fill(alive ? "Alive" : "Deceased");
            statusDescriptor.SetColor(alive ? Color.green : Color.red);
        }

        public override void Clear()
        {
            foreach(var component in componentList)
            {
                component.Clear();
            }
        }

        private void Awake()
        {
            componentList = new List<IWikiComponent>();
            var fieldInfos = GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var matchingFieldInfos = fieldInfos.Where(x => typeof(IWikiComponent).IsAssignableFrom(x.FieldType)).ToList();
            foreach (var fieldInfo in matchingFieldInfos)
            {
                var value = (IWikiComponent)fieldInfo.GetValue(this);
                if (value != null)
                {
                    componentList.Add(value);
                }
            }
        }
    }
}
