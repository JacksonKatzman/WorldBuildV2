using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public interface IContextDeployer
	{
		void Deploy(IIncidentContext context);
		public void UpdateContextIDs(Dictionary<int, IIncidentActionField> updates);
	}
}