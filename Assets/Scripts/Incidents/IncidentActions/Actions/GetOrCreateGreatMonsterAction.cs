using Game.Creatures;
using Game.Simulation;
using Sirenix.OdinInspector;

namespace Game.Incidents
{
	public class GetOrCreateGreatMonsterAction : GetOrCreateAction<GreatMonster>
	{
		public bool useSpecificMonster;

		[ShowIf("@this.useSpecificMonster")]
		public ScriptableObjectRetriever<MonsterData> monsterData;
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
			var monsterToCreate = useSpecificMonster ? monsterData.RetrieveObject() : retrievedMonsterData;
			var createdMonster = transformCharacter ? new GreatMonster(monsterToCreate) : new GreatMonster(monsterToCreate);
			
			return createdMonster;
		}

		protected override void Complete()
		{
			if (madeNew)
			{
				var createdMonster = actionField.GetTypedFieldValue();

				createdMonster.AffiliatedFaction = faction.GetTypedFieldValue();

				if (transformCharacter)
				{
					createdMonster.TransformFrom(personToTransform.GetTypedFieldValue());
				}
				else
				{
					if(createdMonster.AffiliatedFaction == null)
					{
						createdMonster.CharacterName = FlavorService.Instance.genericMonsterNamingTheme.GenerateName(Enums.Gender.ANY);
					}
					else
					{
						createdMonster.CharacterName = createdMonster.AffiliatedFaction.namingTheme.GenerateName(Enums.Gender.ANY);
					}
				}
				ContextDictionaryProvider.AddContext(createdMonster);
			}
		}

		protected override bool VersionSpecificVerify(IIncidentContext context)
		{
			if (OnlyCreate)
			{
				if (!useSpecificMonster)
				{
					retrievedMonsterData = criteria.GetMonsterData();
				}
				else
				{
					retrievedMonsterData = monsterData.RetrieveObject();
				}

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