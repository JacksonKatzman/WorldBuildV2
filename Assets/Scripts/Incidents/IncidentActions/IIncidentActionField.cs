using System;

namespace Game.Incidents
{
	public interface IIncidentActionField 
	{
		int ActionFieldID { get; set; }
		string ActionFieldIDString { get; }
		string NameID { get; set; }

		Type ContextType { get; }

		bool CalculateField(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction);
		IIncidentContext GetFieldValue();
	}
}