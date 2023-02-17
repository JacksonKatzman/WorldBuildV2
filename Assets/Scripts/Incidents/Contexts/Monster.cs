using Game.Creatures;
using System;

namespace Game.Incidents
{
	public class Monster : InertIncidentContext
	{
		public override Type ContextType => typeof(Monster);
		public MonsterData monsterData;
	}
}