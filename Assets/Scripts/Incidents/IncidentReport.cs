using Game.Simulation;
using Game.Utilities;
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
		private Dictionary<string, string> flavors;

		public IncidentReport() { }
		public IncidentReport(int incidentID, int parentID, int year)
		{
			IncidentID = incidentID;
			ParentID = parentID;
			ReportYear = year;
			flavors = new Dictionary<string, string>();
		}

		public void AddLog(string log)
		{
			if(logs == null)
			{
				logs = new Stack<string>();
			}

			logs.Push(log);
		}

		public void AddFlavor(string key, string value)
		{
			if(flavors == null)
			{
				flavors = new Dictionary<string, string>();
			}

			flavors.Add(key, value);
		}

		public void CreateFullLog()
		{
			var fullLog = logs.Pop();
			while(logs.Count > 0)
			{
				fullLog += " " + logs.Pop();
			}

			ReportLog = fullLog;
			ReportLog = GenerateLinkedLog();
		}

		public string GenerateLinkedLog()
		{
			var textLine = string.Copy(ReportLog);

			foreach(var pair in flavors)
			{
				textLine = textLine.Replace(pair.Key, pair.Value);
			}

			textLine = FlavorService.Instance.GenerateFlavor(textLine);

			var matches = Regex.Matches(textLine, @"\{(\d+)\}");

			foreach (Match match in matches)
			{
				var matchString = match.Value;
				var linkedContext = Contexts[matchString];
				var linkString = string.Format("<link=\"{0}\">{1}</link>", linkedContext.ID, linkedContext.Name);
				textLine = textLine.Replace(matchString, linkString);
			}

			matches = Regex.Matches(textLine, @"\{SUBP:(\d+)\}");

			foreach (Match match in matches)
			{
				var matchString = match.Value;
				var matchId = match.Groups[1].Value;
				var keyString = "{" + matchId + "}";
				var linkedContext = Contexts[keyString];

				var pronounString = "SUBJECT-PRONOUN";
				if(linkedContext.GetType() == typeof(Character))
				{
					var character = linkedContext as Character;
					pronounString = StaticFlavorCollections.subjectGenderPronouns[character.Gender];
				}
				else
				{
					pronounString = "it";
				}

				var linkString = string.Format("<link=\"{0}\">{1}</link>", linkedContext.ID, pronounString);
				textLine = textLine.Replace(matchString, linkString);
			}

			matches = Regex.Matches(textLine, @"\{OBJP:(\d+)\}");

			foreach (Match match in matches)
			{
				var matchString = match.Value;
				var matchId = match.Groups[1].Value;
				var keyString = "{" + matchId + "}";
				var linkedContext = Contexts[keyString];

				var pronounString = "OBJECT-PRONOUN";
				if (linkedContext.GetType() == typeof(Character))
				{
					var character = linkedContext as Character;
					pronounString = StaticFlavorCollections.objectGenderPronouns[character.Gender];
				}
				else
				{
					pronounString = "it";
				}

				var linkString = string.Format("<link=\"{0}\">{1}</link>", linkedContext.ID, pronounString);
				textLine = textLine.Replace(matchString, linkString);
			}

			matches = Regex.Matches(textLine, @"\{POSP:(\d+)\}");

			foreach (Match match in matches)
			{
				var matchString = match.Value;
				var matchId = match.Groups[1].Value;
				var keyString = "{" + matchId + "}";
				var linkedContext = Contexts[keyString];

				var pronounString = "POS-PRONOUN";
				if (linkedContext.GetType() == typeof(Character))
				{
					var character = linkedContext as Character;
					pronounString = StaticFlavorCollections.possessiveGenderPronouns[character.Gender];
				}
				else
				{
					pronounString = "its";
				}

				var linkString = string.Format("<link=\"{0}\">{1}</link>", linkedContext.ID, pronounString);
				textLine = textLine.Replace(matchString, linkString);
			}

			textLine = textLine.CapitalizeAfterLink();
			return textLine;
		}

		public void LoadContextProperties(Dictionary<string, int> buffer)
		{
			Contexts = new Dictionary<string, IIncidentContext>();
			foreach(var pair in buffer)
			{
				Contexts.Add(pair.Key, SimulationManager.Instance.AllContexts.GetContextByID(pair.Value));
			}
		}
	}
}