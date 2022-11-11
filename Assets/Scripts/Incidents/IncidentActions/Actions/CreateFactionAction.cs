using Game.Simulation;

namespace Game.Incidents
{
	public class CreateFactionAction : GenericIncidentAction
	{
        public ActionResultField<Faction> factionResult;

        public IntegerRange population;
        public IntegerRange influence;
        public IntegerRange wealth;
        public IntegerRange politicalPriority;
        public IntegerRange economicPriority;
        public IntegerRange religiousPriority;
        public IntegerRange militaryPriority;

        public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
            var faction = new Faction(population, influence, wealth, politicalPriority, economicPriority, religiousPriority, militaryPriority);
            faction.AttemptExpandBorder(1);
            faction.CreateStartingCity();
            factionResult.SetValue(faction);
            SimulationManager.Instance.world.AddContext(faction);
		}
	}
}