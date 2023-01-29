using Game.Simulation;
using Sirenix.OdinInspector;

namespace Game.Incidents
{
	abstract public class GetOrCreateAction<T> : GenericIncidentAction where T : IIncidentContext
	{
		public bool findFirst = true;
		public bool allowCreate = true;
		protected bool madeNew;
		protected bool OnlyCreate => !findFirst && allowCreate;

		[ShowIf("@this.findFirst")]
		public ContextualIncidentActionField<T> actionField;

		public override bool VerifyAction(IIncidentContext context)
		{
			var verified = false;
			madeNew = false;
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
					madeNew = true;
				}
			}
			else if(!findFirst && allowCreate)
			{
				verified = VersionSpecificVerify(context);
				if(!verified)
				{
					return false;
				}
				actionField.value = MakeNew();
				madeNew = true;
			}

			return verified;
		}

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			Complete();
		}

		virtual protected void Complete()
		{
			if (madeNew)
			{
				SimulationManager.Instance.world.AddContext(actionField.GetTypedFieldValue());
			}
		}

		virtual protected bool VersionSpecificVerify(IIncidentContext context)
		{
			return true;
		}

		abstract protected T MakeNew();
	}
}