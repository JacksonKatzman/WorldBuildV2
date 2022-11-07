namespace Game.Incidents
{
	public class FactionEvaluator : ContextEvaluator<Faction>
	{
		protected override Faction GetContext(IIncidentContext context)
		{
			/*
			if (context.ContextType == typeof(Faction))
			{
				return (Faction)context;
			}
			else if (context.ContextType == typeof(Person))
			{
				return ((Person)context).AffiliatedFaction;
			}
			else
			{
				return null;
			}
			*/

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