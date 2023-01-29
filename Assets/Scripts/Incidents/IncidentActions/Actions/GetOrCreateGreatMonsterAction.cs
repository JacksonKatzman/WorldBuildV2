using Game.Creatures;
using Game.Simulation;
using Sirenix.OdinInspector;

namespace Game.Incidents
{
	public class GetOrCreateGreatMonsterAction : GetOrCreateAction<GreatMonster>
	{
		public bool useSpecificMonster;

		[ShowIf("@this.useSpecificMonster")]
		public MonsterData monster;
		[ShowIf("@!this.useSpecificMonster")]
		public MonsterCriteria criteria = new MonsterCriteria();

		[ShowIf("@this.OnlyCreate")]
		public bool transformPerson;
		[ShowIf("@this.transformPerson")]
		public ContextualIncidentActionField<Person> personToTransform;
		protected override GreatMonster MakeNew()
		{
			var monsterToCreate = useSpecificMonster ? monster : criteria.GetMonsterData();
			return transformPerson ? new GreatMonster(monsterToCreate,  personToTransform.GetTypedFieldValue()) : new GreatMonster(monsterToCreate);
		}

		protected override bool VersionSpecificVerify(IIncidentContext context)
		{
			if (OnlyCreate && transformPerson)
			{
				return personToTransform.CalculateField(context);
			}
			else
			{
				return base.VersionSpecificVerify(context);
			}
		}
	}
}