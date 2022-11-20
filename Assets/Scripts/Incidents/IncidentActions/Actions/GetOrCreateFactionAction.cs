using Game.Simulation;
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