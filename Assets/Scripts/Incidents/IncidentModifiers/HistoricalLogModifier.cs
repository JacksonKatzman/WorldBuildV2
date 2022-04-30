using System.Collections.Generic;

namespace Game.Incidents
{
	public class HistoricalLogModifier : IncidentModifier
	{
		public List<string> history;
		public HistoricalLogModifier(List<IIncidentTag> tags, float probability) : base(tags, probability) { }

		public override void LogModifier()
		{
			incidentLogs.AddRange(history);
		}
	}
}