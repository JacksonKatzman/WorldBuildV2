﻿namespace Game.Incidents
{
	public class LocationEvaluator : ContextEvaluator<Location>
	{
		protected override Location GetContext(IIncidentContext context)
		{
			if (context.ContextType.IsAssignableFrom(typeof(ILocationAffiliated)))
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
		}
	}
}