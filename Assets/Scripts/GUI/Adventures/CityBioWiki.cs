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
        [SerializeField]
        private BioDescriptorUI descriptionText;
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

        [SerializeField]
        private FamiliarityRequirementUIToggle charactersToggle;
        [SerializeField]
        private FamiliarityRequirementUIToggle landmarkToggle;
        [SerializeField]
        private FamiliarityRequirementUIToggle flavorToggle;

        protected override void Fill(City value)
        {
            Clear();
            nameText.Fill(value.Name);
            //add relative location within faction to description
            descriptionText.Fill(value.Description);
            populationText.Fill($"Population: {value.Population}");
            //rulersText.Fill()
            knownCharactersText.FillWithContextList(new List<IIncidentContext>(value.Characters));
            charactersToggle.SetEmpty(rulersText.IsEmpty && knownCharactersText.IsEmpty);

            landmarksText.FillWithContextList(new List<IIncidentContext>(value.Landmarks));
            landmarkToggle.SetEmpty(landmarksText.IsEmpty);
            //flavor stuff
            flavorToggle.SetEmpty(flavorText.IsEmpty);
        }

        public void CenterOnObject()
        {
            HexMapCamera.PanToCell(Value.CurrentLocation.GetHexCell());
        }
    }
}
