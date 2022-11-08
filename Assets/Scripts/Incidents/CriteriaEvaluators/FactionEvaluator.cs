﻿namespace Game.Incidents
{
	public class FactionEvaluator : ContextEvaluator<Faction>
	{
		protected override Faction GetContext(IIncidentContext context)
		{
			if (context.ContextType.IsAssignableFrom(typeof(IFactionAffiliated)))
			{
				return ((IFactionAffiliated)context).AffiliatedFaction;
			}
			else
			{
				return null;
			}
		}
	}
}