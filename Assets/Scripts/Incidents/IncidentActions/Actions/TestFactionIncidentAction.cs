using System;
using System.Collections.Generic;
using Game.Factions;
using Sirenix.OdinInspector;

namespace Game.Incidents
{
	[Serializable]
	public class TestFactionIncidentAction : ContextualIncidentAction<Faction>
	{
		[HideReferenceObjectPicker]
		public IncidentContextActionField<Faction> requiredFaction;

		override public void PerformAction(IIncidentContext context)
		{
			PerformDebugAction();
		}
/*
		protected override bool VerifyContextActionFields(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			var succeeded = requiredFaction.CalculateField(context, delayedCalculateAction);
			return succeeded;
		}
*/
		private void PerformDebugAction()
		{
			OutputLogger.Log("Faction Debug Action Performed!");
		}
/*
		public override void UpdateEditor()
		{
			requiredFaction = new IncidentContextActionField<FactionContext>(ContextType);
			requiredFaction.criteria = new List<IIncidentCriteria>();
		}
*/
	}
}