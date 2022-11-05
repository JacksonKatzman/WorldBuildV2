namespace Game.Incidents
{
	public class FactionEvaluator : ContextEvaluator<Faction, Person>
	{
		protected override Faction GetContext(IIncidentContext context)
		{
			if (context.ContextType == typeof(Faction))
			{
				return (Faction)context;
			}
			else if (context.ContextType == typeof(Person))
			{
				return ((Person)context).Faction;
			}
			else
			{
				return null;
			}
		}
	}
}