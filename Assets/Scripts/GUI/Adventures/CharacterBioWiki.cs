using Game.Enums;
using Game.GUI.Wiki;
using Game.Incidents;
using Game.Simulation;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class CharacterBioWiki : StatBlockWikiComponent<Character>
    {
        override public void UpdateByFamiliarity(ContextFamiliarity familiarity)
        {
            foreach(var component in componentList)
            {
                component.UpdateByFamiliarity(familiarity);
            }

            foreach(var toggle in toggles)
            {
                toggle.Toggle(familiarity);
            }
        }

        protected override ContextFamiliarity GetContextFamiliarity()
        {
            return AdventureService.Instance.IsDungeonMasterView ? ContextFamiliarity.TOTAL : value.Familiarity;
        }

        protected override Data.MonsterData GetStatBlock()
        {
            return value.AffiliatedRace.racePreset.baseStatBlock;
        }

        protected override string GetName()
        {
            return value.CharacterName.GetFullName();
        }

        protected override string GetDescription()
        {
            return $"{value.Age.ToString()}, {value.Gender.ToString()}, {Link(value.AffiliatedRace)} of {Link(value.AffiliatedFaction)}";
        }

        protected override Data.SerializableStatBlock GetStats()
        {
            return new Data.SerializableStatBlock(value.Strength, value.Dexterity, value.Constitution, value.Intelligence, value.Wisdom, value.Charisma, 0);
        }

        protected override List<TooltipBox> GetTraits()
        {
            var alive = ContextDictionaryProvider.CurrentContexts[typeof(Character)].Contains(value);
            var statusTrait = Instantiate(tooltipBoxPrefab, traitAnchor);
            var tempTraits = new List<TooltipBox>();
            statusTrait.SetTooltip(alive ? "Alive" : "Deceased", alive ? "They are still living." : "They have passed on.");
            statusTrait.SetColor(alive ? Color.green : Color.red);
            tempTraits.Add(statusTrait);

            foreach (var trait in value.CharacterTraits)
            {
                var traitBox = Instantiate(tooltipBoxPrefab, traitAnchor);
                traitBox.SetTooltip(trait.traitName, trait.traitDescription);
                tempTraits.Add(traitBox);
            }

            return tempTraits;
        }

        protected override List<string> GetDetails()
        {
            var details = new List<string>();
            return details;
        }
    }
}
