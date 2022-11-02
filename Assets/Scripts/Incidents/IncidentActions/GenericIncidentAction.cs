using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	abstract public class GenericIncidentAction : IncidentAction
	{
		
	}

	public class BranchingAction : GenericIncidentAction
	{
		[ListDrawerSettings(CustomAddFunction = "AddBranch"), HideReferenceObjectPicker]
		public List<IncidentActionBranch> branches;

		public BranchingAction()
		{
			branches = new List<IncidentActionBranch>();
		}
		public override void PerformAction(IIncidentContext context)
		{
			throw new NotImplementedException();
		}

		public override void UpdateEditor()
		{
			base.UpdateEditor();

		}

		private void AddBranch()
		{
			branches.Add(new IncidentActionBranch());
		}
	}

	public class IncidentActionBranch
	{
		public int weight;
		public List<IIncidentAction> actions;
	}
}