using System;

namespace Game.Incidents
{
	public interface IIncidentActionField 
	{
		int ActionFieldID { get; set; }
		string ActionFieldIDString { get; }
		string NameID { get; set; }
		string PreviousField { get; set; }
		int PreviousFieldID { get; set; }

		Type ContextType { get; }

		bool CalculateField(IIncidentContext context);
		IIncidentContext GetFieldValue();
	}
}