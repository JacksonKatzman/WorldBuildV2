using Game.Generators.Items;
using Game.GUI.Wiki;
using Game.Incidents;
using Game.Simulation;
using Game.Terrain;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class FactionBioWiki : WikiComponent<Faction>
    {
        //dont forget to include special factions in here if possible

        [SerializeField]
        private BioDescriptorUI nameText;
        [SerializeField]
        private BioDescriptorUI descriptionText;
        [SerializeField]
        private BioDescriptorUI governmentText;
        [SerializeField]
        private BioDescriptorUI influenceText;
        [SerializeField]
        private BioDescriptorUI wealthText;
        [SerializeField]
        private BioDescriptorUI militaryText;
        [SerializeField]
        private BioDescriptorUI factionRelationsText;
        [SerializeField]
        private BioDescriptorUI charactersText;
        [SerializeField]
        private BioDescriptorUI citiesText;
        [SerializeField]
        private BioDescriptorUI landmarksText;
        [SerializeField]
        private BioDescriptorUI organizationsText;

        [SerializeField]
        private FamiliarityRequirementUIToggle governmentToggle;
        [SerializeField]
        private FamiliarityRequirementUIToggle charactersToggle;
        [SerializeField]
        private FamiliarityRequirementUIToggle citiesToggle;
        [SerializeField]
        private FamiliarityRequirementUIToggle landmarkToggle;
        [SerializeField]
        private FamiliarityRequirementUIToggle organizationsToggle;
        [SerializeField]
        private FamiliarityRequirementUIToggle flavorToggle;

        [SerializeField]
        protected Transform traitAnchor;
        [SerializeField]
        protected TooltipBox tooltipBoxPrefab;

        protected List<TooltipBox> traits = new List<TooltipBox>();
        //description: {size} {wealth} {predominate race} Faction
        //a button that centers the camera on the location
        //ruler and government type
        //traits
        //relative influence
        //relative military power
        //relative wealth
        //faction relations
        //notable people
        //cities
        //landmarks
        //organizations
        protected override void Fill(Faction value)
        {
            Clear();
            nameText.Fill(value.Name);
            descriptionText.Fill(value.Description);

            traits = GetTraits();

            if (value.Government != null)
            {
                governmentText.Fill($"{value.Government.template.organizationTemplateName} ruled by:\n");
                var contexts = new List<IIncidentContext>();
                foreach (var leader in value.Government.Leaders)
                {
                    if (leader is Character character)
                    {
                        contexts.Add(character);
                    }
                }
                governmentText.FillWithContextList(contexts);
            }

            var factions = new List<Faction>(ContextDictionaryProvider.GetCurrentContexts<Faction>());
            if(factions.Contains(Value))
            {
                var influence = factions.OrderByDescending(x => x.Influence).ToList();
                influenceText.Fill($"Influence Ranking: {influence.IndexOf(Value)}");
                var wealth = factions.OrderByDescending(x => x.Wealth).ToList();
                wealthText.Fill($"Wealth Ranking: {wealth.IndexOf(Value)}");
                var military = factions.OrderByDescending(x => x.MilitaryPower).ToList();
                militaryText.Fill($"Military Ranking: {military.IndexOf(Value)}");
            }

            foreach(var pair in value.FactionRelations)
            {
                var status = value.FactionsAtWarWith.Contains(pair.Key) ? "War" : "Peace";
                factionRelationsText.Append($"{pair.Key.Link()} : {status}\n");
            }
            factionRelationsText.Trim();

            charactersText.FillWithContextList(new List<IIncidentContext>(ContextDictionaryProvider.GetCurrentContexts<Character>().Where(x => x.AffiliatedFaction == value).ToList()));
            citiesText.FillWithContextList(new List<IIncidentContext>(ContextDictionaryProvider.GetCurrentContexts<City>().Where(x => x.AffiliatedFaction == value).ToList()));
            landmarksText.FillWithContextList(new List<IIncidentContext>(ContextDictionaryProvider.GetCurrentContexts<Landmark>().Where(x => x.AffiliatedFaction == value).ToList()));
            organizationsText.FillWithContextList(new List<IIncidentContext>(ContextDictionaryProvider.GetCurrentContexts<Organization>().Where(x => x.AffiliatedFaction == value).ToList()));

            charactersToggle.SetEmpty(charactersText.IsEmpty);
            citiesToggle.SetEmpty(citiesText.IsEmpty);
            landmarkToggle.SetEmpty(landmarksText.IsEmpty);
            organizationsToggle.SetEmpty(organizationsText.IsEmpty);
        }

        public override void Clear()
        {
            base.Clear();

            foreach (var trait in traits)
            {
                Destroy(trait.gameObject);
            }

            traits.Clear();
        }

        protected List<TooltipBox> GetTraits()
        {
            var alive = ContextDictionaryProvider.CurrentContexts[typeof(Faction)].Contains(Value) 
                || ContextDictionaryProvider.CurrentContexts[typeof(SpecialFaction)].Contains(Value);
            var statusTrait = Instantiate(tooltipBoxPrefab, traitAnchor);
            var tempTraits = new List<TooltipBox>();
            statusTrait.SetTooltip(alive ? "Stable" : "Destroyed", alive ? "Exists to this day." : "Has fallen to ruin.");
            statusTrait.SetColor(alive ? Color.green : Color.red);
            tempTraits.Add(statusTrait);

            foreach (var trait in Value.FactionTraits)
            {
                var traitBox = Instantiate(tooltipBoxPrefab, traitAnchor);
                traitBox.SetTooltip(trait.traitName, trait.traitDescription);
                tempTraits.Add(traitBox);
            }

            return tempTraits;
        }

        public void CenterOnObject()
        {
            HexMapCamera.PanToCell(Value.Capitol.CurrentLocation.GetHexCell());
        }
    }
}
