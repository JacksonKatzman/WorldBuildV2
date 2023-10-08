using Game.Simulation;
using Sirenix.OdinInspector;

namespace Game.Incidents
{
	abstract public class GetOrCreateAction<T> : GenericIncidentAction where T : IIncidentContext
	{
		public bool findFirst = true;
		[OnValueChanged("OnAllowCreateValueChanged")]
		public bool allowCreate = true;
		protected bool madeNew;
		protected bool OnlyCreate => !findFirst && allowCreate;

		[ShowIf("@this.findFirst")]
		public ContextualIncidentActionField<T> actionField;

		[ReadOnly, ShowInInspector]
		public string ResultID => actionField?.ActionFieldIDString;

		public override bool VerifyAction(IIncidentContext context)
		{
			var verified = false;
			madeNew = false;
			if(findFirst && !allowCreate)
			{
				verified = actionField.CalculateField(context);
			}
			else if(findFirst && allowCreate)
			{
				verified = actionField.CalculateField(context);
				if (!verified)
				{
					verified = VersionSpecificVerify(context);
					if (!verified)
					{
						return false;
					}
					actionField.value = MakeNew();
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
				ContextDictionaryProvider.AddContext(actionField.GetTypedFieldValue());
			}
		}

		virtual protected bool VersionSpecificVerify(IIncidentContext context)
		{
			return true;
		}

		virtual protected void OnAllowCreateValueChanged()
		{

		}

		abstract protected T MakeNew();
	}
}