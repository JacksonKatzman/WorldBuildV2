using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class IncidentActionBranch
	{
		public Type ContextType { get; set; }

		[HideReferenceObjectPicker]
		public IncidentActionBranchWeightModifier weightModifier;

		[HideReferenceObjectPicker]
		public IncidentActionHandlerContainer actionHandler;

		public IncidentActionBranch() { }
		public IncidentActionBranch(Type type)
		{
			ContextType = type;
			weightModifier = new IncidentActionBranchWeightModifier(type);
			actionHandler = new IncidentActionHandlerContainer(type);
		}

		public bool VerifyActions(IIncidentContext context)
		{
			return actionHandler.VerifyActions(context) && weightModifier.container.actionField.CalculateField(context);
		}

		public void PerformActions(IIncidentContext context, ref IncidentReport report)
		{
			actionHandler.PerformActions(context, ref report);
		}

		public void UpdateActionFieldIDs(ref int startingValue)
		{
			actionHandler.UpdateActionFieldIDs(ref startingValue);
		}

		public void AddContext(ref IncidentReport report)
		{
			actionHandler.GetContextDictionary(ref report);
		}

		public IIncidentActionField GetContextField(int id)
		{
			return actionHandler.GetContextFromActionFields(id);
		}
	}
}