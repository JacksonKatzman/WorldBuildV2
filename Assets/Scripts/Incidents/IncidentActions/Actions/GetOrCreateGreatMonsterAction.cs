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

		public ContextualIncidentActionField<Faction> faction;

		[ShowIf("@this.OnlyCreate")]
		public bool transformCharacter;
		[ShowIf("@this.transformCharacter")]
		public ContextualIncidentActionField<Character> personToTransform;

		private MonsterData retrievedMonsterData;
		protected override GreatMonster MakeNew()
		{
			var monsterToCreate = useSpecificMonster ? monster : retrievedMonsterData;
			var createdMonster = transformCharacter ? new GreatMonster(monsterToCreate, personToTransform.GetTypedFieldValue()) : new GreatMonster(monsterToCreate);
			createdMonster.AffiliatedFaction = faction.GetTypedFieldValue();
			createdMonster.CharacterName = createdMonster.AffiliatedFaction.namingTheme.GenerateName(Enums.Gender.ANY);
			
			return createdMonster;
		}

		protected override bool VersionSpecificVerify(IIncidentContext context)
		{
			if (OnlyCreate)
			{
				retrievedMonsterData = criteria.GetMonsterData();

				if (retrievedMonsterData == null)
				{
					return false;
				}

				var factionFound = faction.CalculateField(context);
				return transformCharacter ? factionFound && personToTransform.CalculateField(context) : factionFound;
			}
			else
			{
				return base.VersionSpecificVerify(context);
			}
		}
	}
}