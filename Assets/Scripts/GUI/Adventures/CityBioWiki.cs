using Game.Incidents;
using Game.Terrain;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class CityBioWiki : WikiComponent<City>
    {
        [SerializeField]
        private BioDescriptorUI nameText;
        //description: {wealth denomination?} {denomination} in {relative direction?} {faction}
        [SerializeField]
        private BioDescriptorUI descriptionText;
        //a button that centers the camera on the location
        [SerializeField]
        private BioDescriptorUI populationText;
        [SerializeField]
        private BioDescriptorUI rulersText;
        [SerializeField]
        private BioDescriptorUI knownCharactersText;
        [SerializeField]
        private BioDescriptorUI landmarksText;
        [SerializeField]
        private BioDescriptorUI flavorText;

        protected override void Fill(City value)
        {
            Clear();
            nameText.Fill(value.Name);
            //add relative location within faction to description
            descriptionText.Fill($"{value.WealthDenominationString} {value.SizeDenominationString} in {Link(value.AffiliatedFaction)}");
            populationText.Fill($"Population: {value.Population}");
            //rulersText.Fill()
            knownCharactersText.FillWithContextList(new List<IIncidentContext>(value.Characters));
            landmarksText.FillWithContextList(new List<IIncidentContext>(value.Landmarks));
            //flavor stuff
        }

        public void CenterOnObject()
        {
            HexMapCamera.PanToCell(Value.CurrentLocation.GetHexCell());
        }
    }
}
