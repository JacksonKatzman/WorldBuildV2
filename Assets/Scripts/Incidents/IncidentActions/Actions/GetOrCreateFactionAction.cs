using Game.Generators.Names;
using Game.Simulation;
using Game.Utilities;
using Sirenix.OdinInspector;

namespace Game.Incidents
{
	public class GetOrCreateFactionAction : GetOrCreateAction<Faction>
	{
        [ShowIf("@this.allowCreate")]
        public IntegerRange population;
        [ShowIf("@this.allowCreate")]
        public IntegerRange influence;
        [ShowIf("@this.allowCreate")]
        public IntegerRange wealth;
        [ShowIf("@this.allowCreate")]
        public IntegerRange politicalPriority;
        [ShowIf("@this.allowCreate")]
        public IntegerRange economicPriority;
        [ShowIf("@this.allowCreate")]
        public IntegerRange religiousPriority;
        [ShowIf("@this.allowCreate")]
        public IntegerRange militaryPriority;
        [ShowIf("@this.allowCreate")]
        public bool createdByCharacter;
        [ShowIf("@this.createdByCharacter")]
        public ContextualIncidentActionField<Character> creator;

        protected override Faction MakeNew()
		{
            var race = (Race)SimRandom.RandomEntryFromList(SimulationManager.Instance.world.CurrentContexts[typeof(Race)]);
            var newFaction = new Faction(population, influence, wealth, politicalPriority, economicPriority, religiousPriority, militaryPriority, race);

            if(createdByCharacter)
			{
                newFaction.namingTheme = new NamingTheme(creator.GetTypedFieldValue().AffiliatedFaction.namingTheme);
			}
            else
			{
                newFaction.namingTheme = FlavorService.Instance.GenerateMonsterFactionNamingTheme();
			}

            return newFaction;
        }

		protected override void Complete()
		{
            if (madeNew)
            {
                var faction = actionField.GetTypedFieldValue();
                faction.AttemptExpandBorder(1);
            }
            
            base.Complete();
		}

        protected override bool VersionSpecificVerify(IIncidentContext context)
        {
            return createdByCharacter ? creator.CalculateField(context) : base.VersionSpecificVerify(context);
        }
    }
}