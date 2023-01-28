using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Game.Incidents
{
	public class IncidentReport
	{
		public int IncidentID { get; set; }
		public int ParentID { get; set; }
		public Dictionary<string, IIncidentContext> Contexts { get; set; }

		public int ReportYear { get; set; }
		public string ReportLog { get; set; }
		private Stack<string> logs;

		public IncidentReport() { }
		public IncidentReport(int incidentID, int parentID, int year)
		{
			IncidentID = incidentID;
			ParentID = parentID;
			ReportYear = year;
		}

		public void AddLog(string log)
		{
			if(logs == null)
			{
				logs = new Stack<string>();
			}

			logs.Push(log);
		}

		public void CreateFullLog()
		{
			var fullLog = logs.Pop();
			while(logs.Count > 0)
			{
				fullLog += " " + logs.Pop();
			}

			ReportLog = fullLog;
		}

		public string GenerateLinkedLog()
		{
			var textLine = string.Copy(ReportLog);
			textLine = FlavorService.Instance.GenerateFlavor(textLine);

			var matches = Regex.Matches(textLine, @"\{(\d+)\}");

			foreach (Match match in matches)
			{
				var matchString = match.Value;
				var linkedContext = Contexts[matchString];
				var linkString = string.Format("<link=\"{0}\">{1}</link>", linkedContext.ID, linkedContext.Name);
				textLine = textLine.Replace(matchString, linkString);
			}

			return textLine;
		}
	}
}