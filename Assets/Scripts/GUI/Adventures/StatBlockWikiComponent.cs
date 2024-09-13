using Game.Data;
using Game.Enums;
using Game.GUI.Wiki;
using Game.Incidents;
using Game.Simulation;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public abstract class StatBlockWikiComponent<T> : WikiComponent<T>
    {
        [SerializeField]
        protected TooltipBox tooltipBoxPrefab;

        [SerializeField]
        protected TMP_Text characterNameText;

        [SerializeField]
        protected BioDescriptorUI basicDescriptor;

        [SerializeField]
        protected TMP_Text acText;
        [SerializeField]
        protected TMP_Text hpText;
        [SerializeField]
        protected TMP_Text speedText;
        [SerializeField]
        protected TMP_Text climbText;
        [SerializeField]
        protected TMP_Text swimText;
        [SerializeField]
        protected TMP_Text flyText;

        [SerializeField]
        protected StatContainerSimplifiedUI strengthStatContainer;
        [SerializeField]
        protected StatContainerSimplifiedUI dexterityStatContainer;
        [SerializeField]
        protected StatContainerSimplifiedUI constitutionStatContainer;
        [SerializeField]
        protected StatContainerSimplifiedUI intelligenceStatContainer;
        [SerializeField]
        protected StatContainerSimplifiedUI wisdomStatContainer;
        [SerializeField]
        protected StatContainerSimplifiedUI charismaStatContainer;

        [SerializeField]
        protected FamiliarityRequirementUIToggle skillsBlock;
        [SerializeField]
        protected TMP_Text savingThrows;
        [SerializeField]
        protected TMP_Text senses;
        [SerializeField]
        protected TMP_Text skills;
        [SerializeField]
        protected TMP_Text languages;

        [SerializeField]
        protected DamageTypeIconContainerUI damageVulnerabilities;
        [SerializeField]
        protected DamageTypeIconContainerUI damageResistances;
        [SerializeField]
        protected DamageTypeIconContainerUI damageImmunities;
        [SerializeField]
        protected ConditionTypeIconContainerUI conditionImmunities;
        [SerializeField]
        protected FamiliarityRequirementUIToggle typesBlock;

        [SerializeField]
        protected TMP_Text abilitiesText;
        [SerializeField]
        protected FamiliarityRequirementUIToggle abilitiesBlock;

        [SerializeField]
        protected TMP_Text actionsText;
        [SerializeField]
        protected TMP_Text legendaryActionsText;
        [SerializeField]
        protected FamiliarityRequirementUIToggle actionsBlock;
        [SerializeField]
        protected TMP_Text legendaryActionsTitle;


        [SerializeField]
        protected Transform traitAnchor;

        protected T value;

        protected List<TooltipBox> traits = new List<TooltipBox>();
        abstract protected ContextFamiliarity GetContextFamiliarity();
        abstract protected MonsterData GetStatBlock();
        abstract protected string GetName();
        abstract protected string GetDescription();
        abstract protected SerializableStatBlock GetStats();
        abstract protected List<TooltipBox> GetTraits();
        abstract protected List<string> GetDetails();

        protected override void Fill(T value)
        {
            Clear();
            this.value = value;

            characterNameText.text = GetName();
            var descriptor = GetDescription();
            basicDescriptor.Fill(descriptor);

            //use this as a fallback in case they dont have a better preset somewhere
            var statBlock = GetStatBlock();
            var stats = GetStats();
            acText.text = statBlock.armorValue.ToString();
            hpText.text = Mathf.Max(statBlock.health + CalculateModifier(stats.constitution), 0).ToString();
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

            strengthStatContainer.Fill(stats.strength);
            dexterityStatContainer.Fill(stats.dexterity);
            constitutionStatContainer.Fill(stats.constitution);
            intelligenceStatContainer.Fill(stats.intelligence);
            wisdomStatContainer.Fill(stats.wisdom);
            charismaStatContainer.Fill(stats.charisma);

            //traits here
            traits = GetTraits();

            //Ideals, Bonds, and Quirks here
            GetDetails();

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

            UpdateByFamiliarity(GetContextFamiliarity());
        }

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

        protected override void Awake()
        {
            base.Awake();
            EventManager.Instance.AddEventHandler<IsDungeonMasterViewChangedEvent>(OnDungeonMasterViewChanges);
        }

        private void OnDungeonMasterViewChanges(IsDungeonMasterViewChangedEvent gameEvent)
        {
            UpdateByFamiliarity(GetContextFamiliarity());
        }

        private int CalculateModifier(int input)
        {
            return (input - 10) / 2;
        }
    }
}
