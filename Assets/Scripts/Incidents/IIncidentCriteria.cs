using System;

namespace Game.Incidents
{
	public interface IIncidentCriteria
	{
		Type Type { get; }
		bool Evaluate(IIncidentContext context);
	}
}