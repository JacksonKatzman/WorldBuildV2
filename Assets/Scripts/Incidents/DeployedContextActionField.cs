using Sirenix.OdinInspector;
using System;

namespace Game.Incidents
{
	public class DeployedContextActionField<T> : ContextualIncidentActionField<T> where T : IIncidentContext
	{
		[ShowInInspector]
		override public string ActionFieldIDString => "None";

		public DeployedContextActionField(Type parentType) : base(parentType)
		{
		}
	}
}