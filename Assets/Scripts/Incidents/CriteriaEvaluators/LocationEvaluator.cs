using System;

namespace Game.Incidents
{
	public class LocationEvaluator : ContextEvaluator<Location, ILocationAffiliated>
	{
		public LocationEvaluator() : base() { }
		public LocationEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }
		protected override Location GetContext(IIncidentContext context)
		{
			if (typeof(ILocationAffiliated).IsAssignableFrom(context.ContextType))
			{
				return ((ILocationAffiliated)context).CurrentLocation;
			}
			else
			{
				return null;
			}
		}

		public override void Setup()
		{
			Comparators = ExpressionHelpers.LocationComparators;
			compareTo = new InterfacedIncidentActionFieldContainer<ILocationAffiliated>();
		}
	}
}