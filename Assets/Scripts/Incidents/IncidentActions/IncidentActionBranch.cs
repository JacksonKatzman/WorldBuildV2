using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class IncidentActionBranch
	{
		public Type ContextType { get; set; }

		public int baseWeight;
		[ListDrawerSettings(CustomAddFunction = "AddModifier"), HideReferenceObjectPicker]
		public List<IncidentActionBranchWeightModifier> modifiers;

		[HideReferenceObjectPicker]
		public IncidentActionHandler actionHandler;

		public IncidentActionBranch() { }
		public IncidentActionBranch(Type type)
		{
			ContextType = type;
			modifiers = new List<IncidentActionBranchWeightModifier>();
			actionHandler = new IncidentActionHandler(type);
		}

		public int GetWeight(IIncidentContext context)
		{
			var totalWeight = baseWeight;
			foreach(var mod in modifiers)
			{
				totalWeight += mod.Evaluate(context);
			}
			return totalWeight;
		}

		public bool VerifyActions(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			return actionHandler.VerifyActions(context, delayedCalculateAction);
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

		private void AddModifier()
		{
			modifiers.Add(new IncidentActionBranchWeightModifier(ContextType));
		}
	}
}