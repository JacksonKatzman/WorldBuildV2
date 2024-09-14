using Game.Incidents;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class LandmarkWiki : WikiComponent<Landmark>
    {
        [SerializeField]
        private LandmarkBioWiki landmarkBioWiki;

        public override void Clear()
        {
            landmarkBioWiki.Clear();
        }

        protected override void Fill(Landmark value)
        {
            landmarkBioWiki.Fill(value);
        }
    }
}
