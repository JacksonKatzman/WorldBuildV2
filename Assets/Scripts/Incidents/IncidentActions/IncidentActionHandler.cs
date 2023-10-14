using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class IncidentActionHandler
	{
		[TypeFilter("GetFilteredTypeList"), OnValueChanged("SetAction"), HideLabel]
		public IIncidentAction incidentAction;

		private Action onSetCallback;

		public IncidentActionHandler() { }
		public IncidentActionHandler(Action onSetCallback)
		{
			this.onSetCallback = onSetCallback;
		}

		public IncidentActionHandler(IIncidentAction action, Action onSetCallback) : this(onSetCallback)
		{
			this.incidentAction = action;
		}

		public bool VerifyAction(IIncidentContext context)
		{
			if (!incidentAction.VerifyAction(context))
			{
				return false;
			}
			return true;
		}

		public void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			incidentAction.PerformAction(context, ref report);
		}

		public void UpdateActionFieldIDs(ref int startingValue)
		{
			incidentAction.UpdateActionFieldIDs(ref startingValue);
		}

		public void AddContext(ref IncidentReport report)
		{
			incidentAction.AddContext(ref report);
		}

		public bool GetContextField(int id, out IIncidentActionField contextField)
		{
			if(incidentAction.GetContextField(id, out contextField))
			{
				return true;
			}

			return false;
		}
#if UNITY_EDITOR
		private void SetAction()
		{
			incidentAction.UpdateEditor();
			IncidentEditorWindow.UpdateActionFieldIDs();
		}

		private IEnumerable<Type> GetFilteredTypeList()
		{
			return IncidentActionHelpers.GetFilteredTypeList(IncidentEditorWindow.ContextType);
		}
#endif
	}
}