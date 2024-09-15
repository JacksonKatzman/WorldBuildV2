using Game.GUI.Wiki;
using Game.Incidents;
using Game.Simulation;
using Game.Terrain;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class LandmarkBioWiki : WikiComponent<Landmark>
    {
        [SerializeField]
        private BioDescriptorUI nameText;
        [SerializeField]
        private BioDescriptorUI descriptionText;
        //a button that centers the camera on the location
        [SerializeField]
        private BioDescriptorUI factionText;
        [SerializeField]
        private BioDescriptorUI organizationsText;
        [SerializeField]
        private BioDescriptorUI inventoryText;
        [SerializeField]
        private BioDescriptorUI flavorText;

        [SerializeField]
        private FamiliarityRequirementUIToggle factionToggle;
        [SerializeField]
        private FamiliarityRequirementUIToggle inventoryToggle;
        [SerializeField]
        private FamiliarityRequirementUIToggle flavorToggle;
        //inventory
        [SerializeField]
        protected Transform traitAnchor;
        [SerializeField]
        protected TooltipBox tooltipBoxPrefab;

        protected List<TooltipBox> traits = new List<TooltipBox>();

        protected override void Fill(Landmark value)
        {
            Clear();
            nameText.Fill(value.Name);
            descriptionText.Fill(value.Description);
            if (value.AffiliatedFaction != null)
            {
                factionText.Fill(value.AffiliatedFaction.Link());
            }
            if (value.AffiliatedOrganization != null)
            {
                organizationsText.Fill(value.AffiliatedOrganization.Link());
            }
            inventoryText.FillWithContextList(new List<IIncidentContext>(value.CurrentInventory.Items));

            traits = GetTraits();
            //flavor

            factionToggle.SetEmpty(factionText.IsEmpty && organizationsText.IsEmpty);
            inventoryToggle.SetEmpty(inventoryText.IsEmpty);
            flavorToggle.SetEmpty(flavorText.IsEmpty);
        }

        protected List<TooltipBox> GetTraits()
        {
            var alive = ContextDictionaryProvider.CurrentContexts[typeof(Landmark)].Contains(Value);
            var statusTrait = Instantiate(tooltipBoxPrefab, traitAnchor);
            var tempTraits = new List<TooltipBox>();
            statusTrait.SetTooltip(alive ? "Intact" : "Destroyed", alive ? "Exists to this day." : "Has fallen to ruin.");
            statusTrait.SetColor(alive ? Color.green : Color.red);
            tempTraits.Add(statusTrait);

            foreach (var trait in Value.LandmarkTraits)
            {
                var traitBox = Instantiate(tooltipBoxPrefab, traitAnchor);
                traitBox.SetTooltip(trait.traitName, trait.traitDescription);
                tempTraits.Add(traitBox);
            }

            return tempTraits;
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

        public void CenterOnObject()
        {
            HexMapCamera.PanToCell(Value.CurrentLocation.GetHexCell());
        }
    }
}
