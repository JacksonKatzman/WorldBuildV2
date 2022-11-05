using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.Incidents
{
	[Serializable, HideReferenceObjectPicker]
	public class ActionResultField<T> : ContextualIncidentActionField<T> where T : IIncidentContext
	{
		protected override bool ShowMethodChoice => false;

		[ReadOnly]
		public override ActionFieldRetrievalMethod Method => ActionFieldRetrievalMethod.Random;
		public ActionResultField(Type parentType) : base(parentType)
		{
			this.parentType = parentType;
		}
		public override IIncidentContext GetFieldValue()
		{
			return value;
		}

		public override T GetTypedFieldValue()
		{
			return (T)value;
		}

		public override bool CalculateField(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			return true;
		}

		public void SetValue(IIncidentContext context)
		{
			value = context;
		}
	}
}