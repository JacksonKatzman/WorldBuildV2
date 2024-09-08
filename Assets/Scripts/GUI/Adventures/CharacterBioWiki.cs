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

        /*
        override protected void Fill(Character character)
        {
            Clear();

            characterNameText.text = character.CharacterName.GetFullName();
            var descriptor = $"{character.Age.ToString()}, {character.Gender.ToString()}, {Link(character.AffiliatedRace)} of {Link(character.AffiliatedFaction)}";
            basicDescriptor.Fill(descriptor);

            //use this as a fallback in case they dont have a better preset somewhere
            var statBlock = character.AffiliatedRace.racePreset.baseStatBlock;
            acText.text = statBlock.armorValue.ToString();
            hpText.text = Mathf.Max(statBlock.health + character.Constitution, 0).ToString();
            speedText.text = statBlock.speed.ToString();
            climbText.text = statBlock.climbSpeed.ToString();
            swimText.text = statBlock.swimSpeed.ToString();
            flyText.text = statBlock.flySpeed.ToString();

            speedText.text = $"{statBlock.speed.ToString()} ft.";
            var canClimb = statBlock.climbSpeed > 0 ? statBlock.climbSpeed.ToString() : "N/A";
            var canSwim = statBlock.swimSpeed > 0 ? statBlock.swimSpeed.ToString() : "N/A";
            var canFly = statBlock.flySpeed > 0 ? statBlock.flySpeed.ToString() : "N/A";

            climbText.text = canClimb;
            swimText.text = canSwim;
            flyText.text = canFly;

            strengthStatContainer.Fill(character.Strength);
            dexterityStatContainer.Fill(character.Dexterity);
            constitutionStatContainer.Fill(character.Constitution);
            intelligenceStatContainer.Fill(character.Intelligence);
            wisdomStatContainer.Fill(character.Wisdom);
            charismaStatContainer.Fill(character.Charisma);

            //traits here
            var alive = ContextDictionaryProvider.CurrentContexts[typeof(Character)].Contains(character);
            var statusTrait = Instantiate(tooltipBoxPrefab, traitAnchor);
            statusTrait.SetTooltip(alive ? "Alive" : "Deceased", alive ? "They are still living." : "They have passed on.");
            statusTrait.SetColor(alive ? Color.green : Color.red);
            traits.Add(statusTrait);

            foreach(var trait in character.CharacterTraits)
            {
                var traitBox = Instantiate(tooltipBoxPrefab, traitAnchor);
                traitBox.SetTooltip(trait.traitName, trait.traitDescription);
                traits.Add(traitBox);
            }

            //Ideals, Bonds, and Quirks here

            //Saving throws etc
            var saves = statBlock.savingThrows.Print(true);
            savingThrows.text = $"Saving Throws: {saves}";
            savingThrows.gameObject.SetActive(saves != string.Empty);

            senses.text = "Senses: ";
            foreach (var sense in statBlock.senses)
            {
                senses.text += $"{sense.senseType} {sense.distance} ft., ";
            }

            senses.text += $"Passive Perception: {statBlock.passivePerception}";

            var printedSkills = statBlock.skills.Print();
            skills.text = $"Skills: {printedSkills}";
            skills.gameObject.SetActive(printedSkills != string.Empty);

            languages.text = $"Languages: {statBlock.languages.Print()}";

            //Damage types and conditions
            damageVulnerabilities.UpdateIcons(statBlock.damageVulnerabilities);
            damageResistances.UpdateIcons(statBlock.damageResistances);
            damageImmunities.UpdateIcons(statBlock.damageImmunities);
            conditionImmunities.UpdateIcons(statBlock.conditionImmunities);

            var allOff = !damageVulnerabilities.gameObject.activeSelf
                && !damageResistances.gameObject.activeSelf
                && !damageImmunities.gameObject.activeSelf
                && !conditionImmunities.gameObject.activeSelf;
            typesBlock.SetEmpty(allOff);

            abilitiesText.text = string.Empty;
            foreach (var ability in statBlock.abilities)
            {
                abilitiesText.text += $"<i><b>{ability.abilityName}.</b></i> {ability.abilityDescription} \n";
            }

            abilitiesBlock.SetEmpty(abilitiesText.text == string.Empty);

            actionsText.text = string.Empty;
            foreach (var creatureAction in statBlock.actions)
            {
                actionsText.text += $"<i><b>{creatureAction.actionName}.</b></i> {creatureAction.actionDescription} \n";
            }

            var weaponAttackRegex = new Regex(@"((Melee|Ranged) Weapon Attack)");
            actionsText.text = weaponAttackRegex.Replace(actionsText.text, m => string.Format("<i>{0}</i>", m.Groups[1].Value));

            legendaryActionsText.text = string.Empty;
            foreach (var creatureAction in statBlock.legendaryActions)
            {
                legendaryActionsText.text += $"<i><b>{creatureAction.actionName}.</b></i> {creatureAction.actionDescription} \n";
            }

            legendaryActionsText.text = weaponAttackRegex.Replace(legendaryActionsText.text, m => string.Format("<i>{0}</i>", m.Groups[1].Value));


            legendaryActionsText.gameObject.SetActive(statBlock.legendaryActions.Count != 0);
            legendaryActionsTitle.gameObject.SetActive(statBlock.legendaryActions.Count != 0);
            actionsBlock.SetEmpty(actionsText.text == string.Empty && legendaryActionsText.text == string.Empty);

            base.Fill(character);
        }
        */
        /*
        public override void Clear()
        {
            foreach (var component in componentList)
            {
                component.Clear();
            }

            foreach (var trait in traits)
            {
                Destroy(trait.gameObject);
            }

            traits.Clear();
        }
        */
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
