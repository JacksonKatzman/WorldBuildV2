using System.Collections.Generic;

namespace Game.Incidents
{
	[System.Serializable]
    public class IncidentCriteriaContainer
    {
        public List<IIncidentCriteria> criteria;

        public IncidentCriteriaContainer() { }
        public IncidentCriteriaContainer(List<IIncidentCriteria> c)
        {
            criteria = c;
        }

        public bool Evaluate(IIncidentContext context)
        {
            foreach (var criterium in criteria)
            {
                if (!criterium.Evaluate(context))
                {
                    return false;
                }
            }

            return true;
        }
    }
}