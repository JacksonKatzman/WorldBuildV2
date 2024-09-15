using Game.Incidents;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class HexCollectionWiki : WikiComponent<HexCollection>
    {
        [SerializeField]
        private HexCollectionBioWiki hexCollectionBioWiki;

        public override void Clear()
        {
            hexCollectionBioWiki.Clear();
        }

        protected override void Fill(HexCollection value)
        {
            hexCollectionBioWiki.Fill(value);
        }
    }
}
