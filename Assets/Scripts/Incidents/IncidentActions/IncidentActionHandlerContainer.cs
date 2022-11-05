﻿using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class IncidentActionHandlerContainer
	{
		public string incidentLog;
		[ShowInInspector, ListDrawerSettings(CustomAddFunction = "AddNewActionContainer"), HideReferenceObjectPicker]
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

		public bool VerifyActions(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction = null)
		{
			providedContext = context;

			if (delayedCalculateAction == null)
			{
				delayedCalculateAction = GetContextFromActionFields;
			}

			foreach (var action in Actions)
			{
				if (!action.VerifyAction(context, delayedCalculateAction))
				{
					OutputLogger.LogWarning("ActionContainer failed to verify action context!");
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

			report.ReportLog += incidentLog;
			GetContextDictionary(ref report);

			foreach (var deployer in Deployers)
			{
				deployer.Deploy(context, GetContextFromActionFields);
			}

			return true;
		}

		public void UpdateActionFieldIDs(ref int startingValue)
		{
			if (startingValue == 0)
			{
				var constant = new ConstantActionField(providedContext);
				IncidentEditorWindow.actionFields.Add(constant);
				startingValue++;
			}

			foreach(var container in Actions)
			{
				container.UpdateActionFieldIDs(ref startingValue);
			}
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
			Actions.Add(new IncidentActionHandler());
		}

		private void AddNewContextDeployer()
		{
			Deployers.Add(new ContextDeployer());
		}
	}
}