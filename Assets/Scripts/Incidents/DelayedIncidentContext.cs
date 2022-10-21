namespace Game.Incidents
{
	public class DelayedIncidentContext
	{
		public IIncidentContext incidentContext;
		public int delayCounter;

		public DelayedIncidentContext(IIncidentContext context, int delay)
		{
			incidentContext = context;
			delayCounter = delay;
		}
	}
}