using Game.Incidents;
using TMPro;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class CityWiki : WikiComponent<City>
    {
        [SerializeField]
        private CityBioWiki cityBioWiki;

        public override void Clear()
        {
            cityBioWiki.Clear();
        }

        protected override void Fill(City value)
        {
            cityBioWiki.Fill(value);
        }
    }
}
