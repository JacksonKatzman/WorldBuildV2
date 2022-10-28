using System.Collections.Generic;

namespace Game.Incidents
{
	public class IncidentReport
	{
		public int IncidentID { get; set; }
		public int ParentID { get; set; }
		public Dictionary<string, IIncidentContext> Contexts { get; set; }

		public string ReportLog { get; set; }

		public IncidentReport() { }
		public IncidentReport(int incidentID, int parentID)
		{
			IncidentID = incidentID;
			ParentID = parentID;
		}
	}
}