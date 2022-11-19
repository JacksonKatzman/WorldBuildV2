using Game.Simulation;

namespace Game.Incidents
{
	public class GetOrCreateFactionAction : GenericIncidentAction
	{
        public ContextualIncidentActionField<Faction> faction;

        public IntegerRange population;
        public IntegerRange influence;
        public IntegerRange wealth;
        public IntegerRange politicalPriority;
        public IntegerRange economicPriority;
        public IntegerRange religiousPriority;
        public IntegerRange militaryPriority;

        public ActionResultField<Faction> factionResult;

        public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
            if (faction.GetTypedFieldValue() == null)
            {
                var newFaction = new Faction(population, influence, wealth, politicalPriority, economicPriority, religiousPriority, militaryPriority);
                newFaction.AttemptExpandBorder(1);
                newFaction.CreateStartingCity();
                factionResult.SetValue(newFaction);
                SimulationManager.Instance.world.AddContext(newFaction);
            }
            else
			{
                factionResult.SetValue(faction.GetTypedFieldValue());
			}
		}
	}
}