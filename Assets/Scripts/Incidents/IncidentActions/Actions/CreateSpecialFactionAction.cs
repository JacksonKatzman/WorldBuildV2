﻿using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class CreateSpecialFactionAction : CreateFactionAction
	{
        public override void PerformAction(IIncidentContext context, ref IncidentReport report)
        {
            var specialFactionType = SpecialFaction.CalculateFactionType(politicalPriority, economicPriority, religiousPriority, militaryPriority);
            var specialFaction = (Faction)Activator.CreateInstance(specialFactionType);
            specialFaction.Population = population;
            specialFaction.Influence = influence;
            specialFaction.Wealth = wealth;
            specialFaction.PoliticalPriority = politicalPriority;
            specialFaction.EconomicPriority = economicPriority;
            specialFaction.ReligiousPriority = religiousPriority;
            specialFaction.MilitaryPriority = militaryPriority;

            SimulationManager.Instance.world.AddContext<Faction>(specialFaction);
        }

        private IEnumerable<Type> GetFilteredTypeList()
        {
            var q = typeof(SpecialFaction).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsGenericTypeDefinition)
                .Where(x => typeof(SpecialFaction).IsAssignableFrom(x));

            return q;
        }
    }
}