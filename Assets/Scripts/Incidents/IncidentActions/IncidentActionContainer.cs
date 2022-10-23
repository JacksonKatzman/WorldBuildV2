using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class IncidentActionContainer
	{
		public string incidentLog;
		public List<IIncidentAction> Actions { get; set; }
		public List<IContextDeployer> Deployers { get; set; }

		public IncidentActionContainer(List<IIncidentAction> actions, List<IContextDeployer> deployers, string log)
		{
			Actions = actions;
			Deployers = deployers;
			incidentLog = log;
		}

		public bool PerformActions(IIncidentContext context, ref IncidentReport report)
		{
			foreach(var action in Actions)
			{
				if(!action.VerifyAction(context, GetProviderFromActionFields))
				{
					OutputLogger.LogWarning("ActionContainer failed to verify action context!");
					return false;
				}
			}

			foreach (var action in Actions)
			{
				action.PerformAction(context);
			}

			report.Providers = GetContextProviderDictionary(context);
			report.ReportLog = incidentLog;

			foreach(var deployer in Deployers)
			{
				deployer.Deploy(context, GetProviderFromActionFields);
			}

			return true;
		}

		public Dictionary<string, IIncidentContextProvider> GetContextProviderDictionary(IIncidentContext context)
		{
			var providerDictionary = new Dictionary<string, IIncidentContextProvider>();

			foreach (var action in Actions)
			{
				var actionType = action.GetType();
				var fields = actionType.GetFields();
				var matchingFields = fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(IncidentContextActionField<>)).ToList();

				foreach (var field in matchingFields)
				{
					var actionField = field.GetValue(action) as IIncidentActionField;
					providerDictionary.Add(actionField.ActionFieldIDString, actionField.GetFieldValue());
				}
			}

			return providerDictionary;
		}

		public IIncidentActionField GetProviderFromActionFields(int actionFieldID)
		{
			foreach (var action in Actions)
			{
				var actionType = action.GetType();
				var fields = actionType.GetFields();
				var matchingFields = fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(IncidentContextActionField<>)).ToList();

				foreach (var field in matchingFields)
				{
					var actionField = field.GetValue(action) as IIncidentActionField;
					if(actionField.ActionFieldID == actionFieldID)
					{
						return actionField;
					}
				}
			}

			return null;
		}
	}
}