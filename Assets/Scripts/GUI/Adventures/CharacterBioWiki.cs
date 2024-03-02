﻿using Game.Incidents;
using Game.Simulation;
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

        override protected void Fill(Character character)
        {
            characterNameText.text = character.CharacterName.GetFullName();
            ageDescriptor.Fill(character.Age.ToString());
            genderDescriptor.Fill(character.Gender.ToString());
            raceDescriptor.Fill(character.AffiliatedRace.racePreset.name);
            factionDescriptor.Fill(character.AffiliatedFaction.Name);
            //will do the rest once I have a way to generate them

            var alive = ContextDictionaryProvider.CurrentContexts[typeof(Character)].Contains(character);
            statusDescriptor.Fill(alive ? "Alive" : "Deceased");
            statusDescriptor.SetColor(alive ? Color.green : Color.red);
        }
    }
}
