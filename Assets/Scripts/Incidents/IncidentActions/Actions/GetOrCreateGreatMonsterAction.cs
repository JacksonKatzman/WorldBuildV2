using Game.Simulation;

namespace Game.Incidents
{
	public class GetOrCreateGreatMonsterAction : GetOrCreateAction<GreatMonster>
	{
		public MonsterCriteria criteria;
		protected override GreatMonster MakeNew()
		{
			return new GreatMonster(criteria.GetMonsterData());
		}
	}
}