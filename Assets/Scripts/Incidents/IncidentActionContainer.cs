using System.Collections.Generic;

namespace Game.Incidents
{
	public class IncidentActionContainer
	{
		public List<IIncidentAction> Actions { get; set; }

		public IncidentActionContainer(List<IIncidentAction> actions)
		{
			Actions = actions;
		}

		public void PerformActions(IIncidentContext context)
		{
			foreach(var action in Actions)
			{
				action.PerformAction(context);
			}
		}
	}
}