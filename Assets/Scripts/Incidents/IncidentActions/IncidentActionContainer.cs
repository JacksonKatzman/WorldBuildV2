using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class IncidentActionContainer
	{
		public string incidentLog;
		public List<IIncidentAction> Actions { get; set; }
		public List<IContextDeployer> Deployers { get; set; }

		private IIncidentContext providedContext;

		public IncidentActionContainer(List<IIncidentAction> actions, List<IContextDeployer> deployers, string log)
		{
			Actions = actions;
			Deployers = deployers;
			incidentLog = log;
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

		public Dictionary<string, IIncidentContext> GetContextDictionary(IIncidentContext context)
		{
			var contextDictionary = new Dictionary<string, IIncidentContext>();
			contextDictionary.Add("{0}", context);

			foreach (var action in Actions)
			{
				var actionType = action.GetType();
				var fields = actionType.GetFields();
				var matchingFields = fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(IncidentContextActionField<>)).ToList();

				foreach (var field in matchingFields)
				{
					var actionField = field.GetValue(action) as IIncidentActionField;
					contextDictionary.Add(actionField.ActionFieldIDString, actionField.GetFieldValue());
				}
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