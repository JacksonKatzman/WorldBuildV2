using Game.Incidents;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class GreatMonsterWiki : WikiComponent<GreatMonster>
    {
        [SerializeField]
        private WikiComponent<GreatMonster> greatMonsterBio;

        public override void Clear()
        {
            greatMonsterBio.Clear();
        }

        protected override void Fill(GreatMonster value)
        {
            greatMonsterBio.Fill(value);
        }
    }
}
