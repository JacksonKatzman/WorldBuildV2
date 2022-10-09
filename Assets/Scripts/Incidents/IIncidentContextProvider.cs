using System;

namespace Game.Incidents
{
	public interface IIncidentContextProvider
	{
		IIncidentContext GetContext();
		void UpdateContext();
		void DeployContext();
	}
}