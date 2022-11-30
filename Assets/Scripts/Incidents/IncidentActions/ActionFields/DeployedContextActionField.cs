using Sirenix.OdinInspector;
using System;

namespace Game.Incidents
{
	public class DeployedContextActionField<T> : ContextualIncidentActionField<T> where T : IIncidentContext
	{
		public DeployedContextActionField(Type parentType) : base(parentType)
		{
		}
	}
}