using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public interface IIncidentAction
	{
		bool VerifyAction(IIncidentContext context, Func<int, IIncidentActionField> delayedAction);
		void PerformAction(IIncidentContext context);
		void UpdateActionFieldIDs(ref int startingValue);
		void AddContext(ref Dictionary<string, IIncidentContext> contextDictionary);
		bool GetContextField(int id, out IIncidentActionField contextField);
		void UpdateEditor();
	}
}