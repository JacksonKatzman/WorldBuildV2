using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class IncidentActionContainer
	{
		[TypeFilter("GetFilteredTypeList"), OnValueChanged("SetAction"), HideLabel]
		public IIncidentAction incidentAction;

		private Action onSetCallback;

		public IncidentActionContainer() { }
		public IncidentActionContainer(Action onSetCallback)
		{
			this.onSetCallback = onSetCallback;
		}

		public IncidentActionContainer(IIncidentAction action, Action onSetCallback) : this(onSetCallback)
		{
			this.incidentAction = action;
		}

		public bool VerifyAction(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			if (!incidentAction.VerifyAction(context, delayedCalculateAction))
			{
				return false;
			}
			return true;
		}

		public void PerformAction(IIncidentContext context)
		{
			incidentAction.PerformAction(context);
		}

		public void UpdateActionFieldIDs(ref int startingValue)
		{
			incidentAction.UpdateActionFieldIDs(ref startingValue);
		}

		public void AddContext(ref Dictionary<string, IIncidentContext> contextDictionary)
		{
			incidentAction.AddContext(ref contextDictionary);
		}

		public bool GetContextField(int id, out IIncidentActionField contextField)
		{
			if(incidentAction.GetContextField(id, out contextField))
			{
				return true;
			}

			return false;
		}

		private void SetAction()
		{
			incidentAction.UpdateEditor();
			IncidentEditorWindow.UpdateActionFieldIDs();
		}

		private IEnumerable<Type> GetFilteredTypeList()
		{
			return IncidentActionHelpers.GetFilteredTypeList(IncidentEditorWindow.ContextType);
		}
	}
}