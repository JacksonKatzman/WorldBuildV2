using System;

namespace Game.Incidents
{
	public class FactionEvaluator : ContextEvaluator<Faction, IFactionAffiliated>
	{
		public FactionEvaluator() : base() { }
		public FactionEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }

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