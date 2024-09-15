using Game.Incidents;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class FactionWiki : WikiComponent<Faction>
    {
        [SerializeField]
        private FactionBioWiki factionBioWiki;

        public override void Clear()
        {
            factionBioWiki.Clear();
        }

        protected override void Fill(Faction value)
        {
            factionBioWiki.Fill(value);
        }
    }
}
