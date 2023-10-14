using Game.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class BranchingAction : GenericIncidentAction
	{
		[ListDrawerSettings(CustomAddFunction = "AddBranch"), HideReferenceObjectPicker]
		public List<IncidentActionBranch> branches;

		public BranchingAction()
		{
		}

		override public bool VerifyAction(IIncidentContext context)
		{
			foreach (var branch in branches)
			{
				if(!branch.VerifyActions(context))
				{
					OutputLogger.LogWarning(String.Format("{0} failed to verify branch {1}.", GetType().Name, branches.IndexOf(branch)));
					return false;
				}
			}

			return true;
		}

		override public void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			var totalWeight = 0;
			foreach (var branch in branches)
			{
				totalWeight += branch.weightModifier.Calculate();
			}
			var randomWeight = SimRandom.RandomRange(1, totalWeight+1);
			for(int i = 0; i < branches.Count; i++)
			{
				randomWeight -= branches[i].weightModifier.Calculate();
				if(randomWeight <= 0)
				{
					branches[i].PerformActions(context, ref report);
					return;
				}
			}
			return;
		}

		override public void UpdateEditor()
		{
			base.UpdateEditor();
			if (branches == null)
			{
				branches = new List<IncidentActionBranch>();
			}
		}

		override public void UpdateActionFieldIDs(ref int startingValue)
		{
			foreach(var branch in branches)
			{
				branch.UpdateActionFieldIDs(ref startingValue);
			}
		}

		override public bool GetContextField(int id, out IIncidentActionField contextField)
		{
			foreach (var branch in branches)
			{
				var test = branch.GetContextField(id);
				if(test != null)
				{
					contextField = test;
					return true;
				}
			}
			contextField = null;
			return false;
		}
#if UNITY_EDITOR
		private void AddBranch()
		{
			branches.Add(new IncidentActionBranch(IncidentEditorWindow.ContextType));
		}
#endif
	}
}