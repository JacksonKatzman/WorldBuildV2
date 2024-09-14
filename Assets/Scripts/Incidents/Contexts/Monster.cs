using Game.Data;
using System;

namespace Game.Incidents
{
	public class Monster : InertIncidentContext
	{
		public override Type ContextType => typeof(Monster);

        public override string Description => monsterData.GetDescription();

        public MonsterData monsterData;
    }
}