using System.Collections.Generic;

namespace Game.Incidents
{
	public class IncidentActionFieldCriteriaContainer
	{
        public List<IncidentActionFieldCriteria> criteria;
        public IncidentActionFieldCriteriaContainer() { }
        public IncidentActionFieldCriteriaContainer(List<IncidentActionFieldCriteria> criteria)
		{
            this.criteria = criteria;
		}

        public bool Evaluate(IIncidentContext context, IIncidentContext parentContext)
		{
            foreach (var criterium in criteria)
            {
                if (!criterium.Evaluate(context, parentContext))
                {
                    return false;
                }
            }

            return true;
        }
    }
}