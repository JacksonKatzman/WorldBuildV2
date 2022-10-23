using System;

namespace Game.Incidents
{
	public interface IContextDeployer
	{
		void Deploy(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction);
	}
}