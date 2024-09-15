using Game.Incidents;
using Game.Terrain;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class HexCollectionBioWiki : WikiComponent<HexCollection>
    {
        [SerializeField]
        private BioDescriptorUI nameText;
        [SerializeField]
        private BioDescriptorUI descriptionText;
        protected override void Fill(HexCollection value)
        {
            nameText.Fill(value.Name);
            descriptionText.Fill(value.Description);
        }

        public void CenterOnObject()
        {
            HexMapCamera.PanToCell(Value.CurrentLocation.GetHexCell());
        }
    }
}
