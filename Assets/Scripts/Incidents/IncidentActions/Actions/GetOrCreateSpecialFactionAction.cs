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
            var specialFaction = (Faction)Activator.CreateInstance(specialFactionType);
            specialFaction.Population = population;
            specialFaction.Influence = influence;
            specialFaction.Wealth = wealth;
            specialFaction.PoliticalPriority = politicalPriority;
            specialFaction.EconomicPriority = economicPriority;
            specialFaction.ReligiousPriority = religiousPriority;
            specialFaction.MilitaryPriority = militaryPriority;

            if (createdByCharacter)
            {
                specialFaction.namingTheme = new NamingTheme(creator.GetTypedFieldValue().AffiliatedFaction.namingTheme);
            }
            else
            {
                specialFaction.namingTheme = FlavorService.Instance.GenerateMonsterFactionNamingTheme();
            }

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