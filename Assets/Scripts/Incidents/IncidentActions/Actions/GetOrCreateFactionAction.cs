﻿using Game.Simulation;
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

		protected override Faction MakeNew()
		{
            var race = (Race)SimRandom.RandomEntryFromList(SimulationManager.Instance.world.CurrentContexts[typeof(Race)]);
            var newFaction = new Faction(population, influence, wealth, politicalPriority, economicPriority, religiousPriority, militaryPriority, race);

            return newFaction;
        }

		protected override void Complete()
		{
            if (madeNew)
            {
                var faction = actionField.GetTypedFieldValue();
                faction.AttemptExpandBorder(1);
                faction.CreateStartingCity();
            }
            
            base.Complete();
		}
	}
}