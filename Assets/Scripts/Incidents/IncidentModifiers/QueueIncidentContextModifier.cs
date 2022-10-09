using System.Collections.Generic;

namespace Game.Incidents
{
	public class QueueIncidentContextModifier : IncidentModifier
	{
		public OldIncidentContext context;
		public int turnDelay;
		public QueueIncidentContextModifier(List<IIncidentTag> tags, float probability) : base(tags, probability) { }

		public override void Finish()
		{
			if(turnDelay > 0)
			{
				OldIncidentService.Instance.QueueDelayedIncident(context, turnDelay);
			}
			else
			{
				OldIncidentService.Instance.PerformIncident(context);
			}
		}
	}
}