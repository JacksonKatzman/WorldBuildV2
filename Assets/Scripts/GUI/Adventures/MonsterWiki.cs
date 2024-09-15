using Game.Data;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class MonsterWiki : WikiComponent<MonsterData>
    {
        [SerializeField]
        private WikiComponent<MonsterData> monsterInfo;

        public override void Clear()
        {
            monsterInfo.Clear();
        }

        protected override void Fill(MonsterData value)
        {
            monsterInfo.Fill(value);
        }
    }
}
