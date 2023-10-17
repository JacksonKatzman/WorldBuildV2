using Game.Data;
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
        public InterfacedIncidentActionFieldContainer<ISentient> creator;

        protected override Faction MakeNew()
		{
            var race = (Race)SimRandom.RandomEntryFromList(ContextDictionaryProvider.CurrentContexts[typeof(Race)]);

            Character factionCreator = creator.contextType == typeof(Character) ? creator.actionField.GetFieldValue() as Character : null;

            var newFaction = new Faction(population, influence, wealth, politicalPriority, economicPriority, religiousPriority, militaryPriority, race, 1, factionCreator);

            return newFaction;
        }

		protected override void Complete()
		{
            if (madeNew)
            {
                var faction = actionField.GetTypedFieldValue();
                faction.namingTheme = new NamingTheme(creator.GetTypedFieldValue().AffiliatedFaction.namingTheme);
                faction.AttemptExpandBorder(1);
                EventManager.Instance.Dispatch(new AddContextEvent(faction));
            }
            
            base.Complete();
		}

        override protected void OnAllowCreateValueChanged()
        {
            creator.enabled = allowCreate;
        }

        protected override bool VersionSpecificVerify(IIncidentContext context)
        {
            return creator.actionField.CalculateField(context) && base.VersionSpecificVerify(context);
        }
    }
}