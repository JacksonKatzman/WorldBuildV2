﻿using Game.Enums;
using Game.Generators.Names;
using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class GetOrCreateSpecialFactionAction : GetOrCreateFactionAction
	{
        [ValueDropdown("GetFilteredTypeList"), LabelText("Special Faction Type")]
        public Type factionType;
        protected override Faction MakeNew()
		{
            var specialFactionType = factionType == null ? SpecialFaction.CalculateFactionType(politicalPriority, economicPriority, religiousPriority, militaryPriority) : factionType;
            var specialFaction = (SpecialFaction)Activator.CreateInstance(specialFactionType);
            specialFaction.Population = population;
            specialFaction.Influence = influence;
            specialFaction.Wealth = wealth;

            specialFaction.Priorities = new Dictionary<OrganizationType, int>();
            specialFaction.Priorities[OrganizationType.POLITICAL] = politicalPriority;
            specialFaction.Priorities[OrganizationType.ECONOMIC] = economicPriority;
            specialFaction.Priorities[OrganizationType.RELIGIOUS] = religiousPriority;
            specialFaction.Priorities[OrganizationType.MILITARY] = militaryPriority;

            specialFaction.namingTheme = new NamingTheme(creator.GetTypedFieldValue().AffiliatedFaction.namingTheme);
            //need to set the leader of the special faction here
            //also need to assign them a base of operations, probably a landmark somehow

            return specialFaction;
        }

        protected override void Complete()
        {
            if (madeNew)
            {
                ContextDictionaryProvider.AddContext(actionField.GetTypedFieldValue());
            }
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