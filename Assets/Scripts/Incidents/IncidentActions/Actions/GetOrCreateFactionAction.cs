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
        public bool useTemplate;
        [ShowIf("@this.CreatingWithTemplate")]
        public ScriptableObjectRetriever<OrganizationTemplate> template;

        [ShowIf("@!this.CreatingWithTemplate")]
        public IntegerRange politicalPriority;
        [ShowIf("@!this.CreatingWithTemplate")]
        public IntegerRange economicPriority;
        [ShowIf("@!this.CreatingWithTemplate")]
        public IntegerRange religiousPriority;
        [ShowIf("@!this.CreatingWithTemplate")]
        public IntegerRange militaryPriority;
        [ShowIf("@this.allowCreate")]
        public InterfacedIncidentActionFieldContainer<ISentient> creator;

        private bool CreatingWithTemplate => allowCreate && useTemplate;

        protected override Faction MakeNew()
		{
            var race = (Race)SimRandom.RandomEntryFromList(ContextDictionaryProvider.CurrentContexts[typeof(Race)]);

            Character factionCreator = creator.contextType == typeof(Character) ? creator.actionField.GetFieldValue() as Character : null;

            if(useTemplate)
			{
                return new Faction(template.RetrieveObject(), 1, population, race, false, factionCreator);
			}
            else
			{
                return new Faction(population, influence, wealth, politicalPriority, economicPriority, religiousPriority, militaryPriority, race, 1, false, factionCreator);
            }
        }

		protected override void Complete()
		{
            if (madeNew)
            {
                var faction = actionField.GetTypedFieldValue();
                faction.namingTheme = new NamingTheme(creator.GetTypedFieldValue().AffiliatedFaction.namingTheme);
                OrganizationTemplate t = useTemplate ? template.RetrieveObject() : null;
                faction.Init(population, 1, t);
                EventManager.Instance.Dispatch(new AddContextEvent(faction, false));
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