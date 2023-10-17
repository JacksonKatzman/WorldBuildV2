using Game.Debug;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public class IncidentActionHandlerContainer
	{
		[TextArea(4,10)]
		public string incidentLog;

		[ShowInInspector, ListDrawerSettings(CustomAddFunction = "AddNewActionContainer", CustomRemoveIndexFunction = "RemoveActionContainer"), HideReferenceObjectPicker]
		public List<IncidentActionHandler> Actions { get; set; }

		[ShowInInspector, ListDrawerSettings(CustomAddFunction = "AddNewContextDeployer"), HideReferenceObjectPicker]
		public List<IContextDeployer> Deployers { get; set; }

		public Type ContextType { get; set; }

		private IIncidentContext providedContext;

		public IncidentActionHandlerContainer() { }
		public IncidentActionHandlerContainer(Type type)
		{
			Actions = new List<IncidentActionHandler>();
			Deployers = new List<IContextDeployer>();
			ContextType = type;
		}

		public bool VerifyActions(IIncidentContext context)
		{
			providedContext = context;

			foreach (var action in Actions)
			{
				if (!action.VerifyAction(context))
				{
					OutputLogger.LogWarning($"ActionContainer failed to verify {context.GetType().Name} : {context.Name} for" +
						$" {action.incidentAction.GetType().Name} as part of {IncidentService.Instance.CurrentIncident.IncidentName}!");
					return false;
				}
			}

			return true;
		}

		public bool PerformActions(IIncidentContext context, ref IncidentReport report)
		{
			providedContext = context;

			foreach (var action in Actions)
			{
				action.PerformAction(context, ref report);
			}

			report.AddLog(incidentLog);
			GetContextDictionary(ref report);

			foreach (var deployer in Deployers)
			{
				deployer.Deploy(context);
			}

			return true;
		}

		public void UpdateActionFieldIDs(ref int startingValue)
		{
#if UNITY_EDITOR
			IncidentEditorWindow.handlerContainers.Add(this);
			if (startingValue == 0)
			{
				var constant = new ConstantActionField(ContextType);
				IncidentEditorWindow.actionFields.Add(constant);
				startingValue++;
			}

			foreach(var container in Actions)
			{
				container.UpdateActionFieldIDs(ref startingValue);
			}

			UpdateFlavorIDs();
#endif
		}

		public void GetContextDictionary(ref IncidentReport report)
		{
			foreach (var action in Actions)
			{
				action.AddContext(ref report);
			}
		}

		public IIncidentActionField GetContextFromActionFields(int actionFieldID)
		{
			if(actionFieldID == 0)
			{
				return new ConstantActionField(providedContext);
			}

			//This is to get DeployedContextActionFields from a DeployedContext
			var deployedContextFields = ActionFieldReflection.GetGenericFieldsByType(providedContext.GetType(), typeof(DeployedContextActionField<>)).ToList();
			for(int i = 0; i < deployedContextFields.Count; i++)
			{
				if(actionFieldID == i+1)
				{
					return (IIncidentActionField)deployedContextFields[i].GetValue(providedContext);
				}
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

		public void UpdatedDeployableContextIDs(Dictionary<int, IIncidentActionField> updates)
		{
			foreach(var deployer in Deployers)
			{
				deployer.UpdateContextIDs(updates);
			}
		}

		public void UpdateFlavorIDs()
		{
			var flavorActions = Actions.Where(x => x.incidentAction.GetType() == typeof(GetFlavorAction)).ToList();
			for(int i = 0; i < flavorActions.Count; i++)
			{
				var flavorAction = flavorActions[i].incidentAction as GetFlavorAction;
				flavorAction.FlavorActionId = i;
			}
		}
#if UNITY_EDITOR
		private void AddNewActionContainer()
		{
			Actions.Add(new IncidentActionHandler());
		}

		private void RemoveActionContainer(int i)
		{
			IncidentEditorWindow.removedIDs.AddRange(Actions[i].incidentAction.GetAllActionFieldIDs());
			Actions.RemoveAt(i);
			IncidentEditorWindow.UpdateActionFieldIDs();
			UpdateFlavorIDs();
		}

		private void AddNewContextDeployer()
		{
			Deployers.Add(new ContextDeployer());
		}
#endif
	}
}