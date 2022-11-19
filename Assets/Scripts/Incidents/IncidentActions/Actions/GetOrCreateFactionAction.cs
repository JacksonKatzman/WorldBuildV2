using Game.Simulation;

namespace Game.Incidents
{
	public class GetOrCreateFactionAction : GetOrCreateAction<Faction>
	{
        public IntegerRange population;
        public IntegerRange influence;
        public IntegerRange wealth;
        public IntegerRange politicalPriority;
        public IntegerRange economicPriority;
        public IntegerRange religiousPriority;
        public IntegerRange militaryPriority;

		protected override void MakeNew()
		{
            var newFaction = new Faction(population, influence, wealth, politicalPriority, economicPriority, religiousPriority, militaryPriority);
            newFaction.AttemptExpandBorder(1);
            newFaction.CreateStartingCity();
            result.SetValue(newFaction);
            SimulationManager.Instance.world.AddContext(newFaction);
        }
	}
}