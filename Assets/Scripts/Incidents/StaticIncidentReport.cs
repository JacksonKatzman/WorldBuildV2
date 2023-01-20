using System.Collections.Generic;

namespace Game.Incidents
{
	public class StaticIncidentReport : IncidentReport
	{
		public StaticIncidentReport() { }
		public StaticIncidentReport(int incidentID, int parentID, int year) : base(incidentID, parentID, year) { }

		public void AddContext(IIncidentContext context)
		{
			if(Contexts == null)
			{
				Contexts = new Dictionary<string, IIncidentContext>();
			}

			var id = "{" + Contexts.Count + "}";
			Contexts.Add(id, context);
		}

		public void AddContexts(List<IIncidentContext> contexts)
		{
			foreach(var context in contexts)
			{
				AddContext(context);
			}
		}
	}
}