using Game.Simulation;
using Sirenix.OdinInspector;

namespace Game.Incidents
{
	abstract public class GetOrCreateAction<T> : GenericIncidentAction where T : IIncidentContext
	{
		public bool findFirst = true;
		public bool allowCreate = true;

		public ContextualIncidentActionField<T> actionField;

		public override bool VerifyAction(IIncidentContext context)
		{
			var verified = false;
			if(findFirst && !allowCreate)
			{
				verified = base.VerifyAction(context);
			}
			else if(findFirst && allowCreate)
			{
				verified = base.VerifyAction(context);
				if(!verified)
				{
					actionField.value = MakeNew();
					verified = true;
				}
			}
			else if(!findFirst && allowCreate)
			{
				actionField.value = MakeNew();
				verified = true;
			}

			return verified;
		}

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			Complete();
		}

		virtual protected void Complete()
		{
			SimulationManager.Instance.world.AddContext(actionField.GetTypedFieldValue());
		}

		abstract protected T MakeNew();
	}
}