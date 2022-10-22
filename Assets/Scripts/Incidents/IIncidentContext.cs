using System;

namespace Game.Incidents
{
	public interface IIncidentContext
	{
		IIncidentContextProvider Provider { get; }
		Type ContextType { get; }
		int NumIncidents { get; }
		int ParentID { get; }
	}
}