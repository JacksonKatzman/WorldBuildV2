using Game.Enums;
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

        public InterfacedIncidentActionFieldContainer<ILocationAffiliated> location;
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

            return specialFaction;
        }

        protected override void Complete()
        {
            if (madeNew)
            {
                var faction = actionField.GetTypedFieldValue() as SpecialFaction;
                var creatorsFaction = creator.GetTypedFieldValue() as IFactionAffiliated;
                if(creatorsFaction.AffiliatedFaction != null && creatorsFaction.AffiliatedFaction.namingTheme != null)
				{
                    faction.namingTheme = new NamingTheme(creatorsFaction.AffiliatedFaction.namingTheme);
                }
                else
				{
                    //replace this with something better later
                    faction.namingTheme = FlavorService.Instance.GenerateMonsterFactionNamingTheme();
                }
                //need to set the leader of the special faction here
                //also need to assign them a base of operations, probably a landmark somehow
                faction.SetLocation(location.GetTypedFieldValue());
                faction.SetCreator(creator.GetTypedFieldValue());
                EventManager.Instance.Dispatch(new AddContextEvent(faction, typeof(Faction)));
            }
        }

        protected override bool VersionSpecificVerify(IIncidentContext context)
        {
            return location.actionField.CalculateField(context) && base.VersionSpecificVerify(context);
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