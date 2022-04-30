using System.Collections.Generic;

namespace Game.Incidents
{
	public class QueueIncidentContextModifier : IncidentModifier
	{
		public IncidentContext context;
		public int turnDelay;
		public QueueIncidentContextModifier(List<IIncidentTag> tags, float probability) : base(tags, probability) { }

		public override void Finish()
		{
			if(turnDelay > 0)
			{
				IncidentService.Instance.QueueDelayedIncident(context, turnDelay);
			}
			else
			{
				IncidentService.Instance.PerformIncident(context);
			}
		}
	}
}