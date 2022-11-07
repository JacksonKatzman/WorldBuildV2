using System;

namespace Game.Incidents
{
	public class IncidentActionFieldCriteria : IncidentCriteria
	{
        public IncidentActionFieldCriteria() { }
        public IncidentActionFieldCriteria(Type contextType) : base(contextType) { }

		protected override bool IsValidPropertyType(Type type)
		{
            return base.IsValidPropertyType(type) || type == typeof(Faction) || type == typeof(Location);
        }

		protected override void SetPrimitiveType()
		{
			base.SetPrimitiveType();

			if (PrimitiveType == typeof(Faction))
			{
				evaluator = new FactionEvaluator();
			}
			else if(PrimitiveType == typeof(Location))
			{
				evaluator = new LocationEvaluator();
			}
		}
	}
}