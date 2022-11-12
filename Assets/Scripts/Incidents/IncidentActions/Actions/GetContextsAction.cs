using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class GetContextsAction : GenericIncidentAction
	{
		[ListDrawerSettings(CustomAddFunction = "AddNewActionFieldContainer", CustomRemoveIndexFunction = "OnRemoveIndex"), HideReferenceObjectPicker]
		public List<IncidentActionFieldContainer> actionFields;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
		}

		private void AddNewActionFieldContainer()
		{
			actionFields.Add(new IncidentActionFieldContainer());
		}

		private void OnRemoveIndex(int i)
		{
			actionFields.RemoveAt(i);
			IncidentEditorWindow.UpdateActionFieldIDs();
		}
	}
}