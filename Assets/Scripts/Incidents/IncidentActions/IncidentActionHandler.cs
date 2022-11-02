using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class IncidentActionHandler
	{
		public string incidentLog;
		[ShowInInspector, ListDrawerSettings(CustomAddFunction = "AddNewActionContainer"), HideReferenceObjectPicker]
		public List<IncidentActionContainer> Actions { get; set; }
		[ShowInInspector, ListDrawerSettings(CustomAddFunction = "AddNewContextDeployer"), HideReferenceObjectPicker]
		public List<IContextDeployer> Deployers { get; set; }
		public Type ContextType { get; set; }

		private IIncidentContext providedContext;

		public IncidentActionHandler() { }
		public IncidentActionHandler(Type type)
		{
			Actions = new List<IncidentActionContainer>();
			Deployers = new List<IContextDeployer>();
			ContextType = type;
		}

		public bool PerformActions(IIncidentContext context, ref IncidentReport report)
		{
			providedContext = context;

			foreach(var action in Actions)
			{
				if(!action.VerifyAction(context, GetContextFromActionFields))
				{
					OutputLogger.LogWarning("ActionContainer failed to verify action context!");
					return false;
				}
			}

			foreach (var action in Actions)
			{
				action.PerformAction(context);
			}

			report.Contexts = GetContextDictionary(context);
			report.ReportLog = incidentLog;

			foreach(var deployer in Deployers)
			{
				deployer.Deploy(context, GetContextFromActionFields);
			}

			return true;
		}

		public void UpdateActionFieldIDs()
		{
			var constant = new ConstantActionField(ContextType);
			IncidentEditorWindow.actionFields.Add(constant);
			int startingValue = 1;
			foreach(var container in Actions)
			{
				container.UpdateActionFieldIDs(ref startingValue);
			}
		}

		//need to refactor these both to grab shit at the right reflection level
		public Dictionary<string, IIncidentContext> GetContextDictionary(IIncidentContext context)
		{
			var contextDictionary = new Dictionary<string, IIncidentContext>();
			contextDictionary.Add("{0}", context);

			foreach (var action in Actions)
			{
				action.AddContext(ref contextDictionary);
			}

			return contextDictionary;
		}

		public IIncidentActionField GetContextFromActionFields(int actionFieldID)
		{
			if(actionFieldID == 0)
			{
				return new ConstantActionField(providedContext.ContextType);
			}

			foreach (var action in Actions)
			{
				if(action.GetContextField(actionFieldID, out var incidentActionField))
				{
					return incidentActionField;
				}
			}

			return null;
		}

		private void AddNewActionContainer()
		{
			Actions.Add(new IncidentActionContainer());
		}

		private void AddNewContextDeployer()
		{
			Deployers.Add(new ContextDeployer());
		}
	}
}