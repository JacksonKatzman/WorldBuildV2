using Sirenix.OdinInspector;
using System;

namespace Game.Incidents
{
	public class DeployedContextActionField<T> : ContextualIncidentActionField<T> where T : IIncidentContext
	{
		[ShowInInspector]
		override public string ActionFieldIDString => ActionFieldID == 0 ? "None" : base.ActionFieldIDString;

		public DeployedContextActionField(Type parentType) : base(parentType)
		{
		}
	}
}