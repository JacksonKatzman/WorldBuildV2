using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public interface IIncidentAction
	{
		bool VerifyAction(IIncidentContext context);
		void PerformAction(IIncidentContext context, ref IncidentReport report);
		void UpdateActionFieldIDs(ref int startingValue);
		void AddContext(ref IncidentReport report);
		bool GetContextField(int id, out IIncidentActionField contextField);
		void UpdateEditor();
	}
}