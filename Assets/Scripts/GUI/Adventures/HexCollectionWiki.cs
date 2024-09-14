using Game.Incidents;
using UnityEngine;

namespace Game.GUI.Adventures
{
    /*
    public class RaceWiki : WikiComponent<Race>
    {

    }

    public class WorldWiki
    {
        //we should replace all incidents with a world wiki that includes the ability to see all incidents (maybe as a tab)
        //but also shows stuff like the name, zones, biome breakdown?, pantheon, etc
    }
    */
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
