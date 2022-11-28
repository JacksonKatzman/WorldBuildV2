namespace Game.Incidents
{
	public class FactionEvaluator : ContextEvaluator<Faction, IFactionAffiliated>
	{
		protected override Faction GetContext(IIncidentContext context)
		{
			if ((typeof(IFactionAffiliated)).IsAssignableFrom(context.ContextType))
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