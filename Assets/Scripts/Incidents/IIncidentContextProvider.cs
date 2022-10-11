using System;

namespace Game.Incidents
{
	public interface IIncidentContextProvider
	{
		IIncidentContext GetContext();
		Type ContextType { get; }
		void UpdateContext();
		void DeployContext();
	}
}