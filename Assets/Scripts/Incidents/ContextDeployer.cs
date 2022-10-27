using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public class ContextDeployer : IContextDeployer
	{
		public int delayTime = 0;

		[ListDrawerSettings(CustomAddFunction = "AddNewCriterium"), HideReferenceObjectPicker]
		public List<DeploymentCriterium> deploymentCriteria;

		[TypeFilter("GetFilteredTypeList"), OnValueChanged("SetContextType"), LabelText("Incident Type")]
		public IDeployableContext incidentContext;

		public ContextDeployer()
		{
			deploymentCriteria = new List<DeploymentCriterium>();
		}

		public void Deploy(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			foreach(var criterium in deploymentCriteria)
			{
				if(!criterium.Evaluate(context, delayedCalculateAction))
				{
					OutputLogger.Log("Failed deployment criteria!");
					return;
				}
			}

			incidentContext.CalculateFields(context, delayedCalculateAction);

			if(delayTime == 0)
			{
				IncidentService.Instance.PerformIncidents(incidentContext);
			}
			else
			{
				IncidentService.Instance.AddDelayedContext(incidentContext, delayTime);
			}

			OutputLogger.Log("Deployed Criteria!");
		}

		private IEnumerable<Type> GetFilteredTypeList()
		{
			var q = typeof(IDeployableContext).Assembly.GetTypes()
				.Where(x => !x.IsAbstract)                                          // Excludes BaseClass
				.Where(x => !x.IsGenericTypeDefinition)                             // Excludes C1<>
				.Where(x => typeof(IDeployableContext).IsAssignableFrom(x));          // Excludes classes not inheriting from BaseClass

			return q;
		}

		private void SetContextType()
		{
			var fields = incidentContext.GetType().GetFields();
			var matchingFields = fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(DeployedContextActionField<>)).ToList();

			foreach (var field in matchingFields)
			{
				field.SetValue(incidentContext, Activator.CreateInstance(field.FieldType, this.GetType()));
			}
		}

		private void AddNewCriterium()
		{
			deploymentCriteria.Add(new DeploymentCriterium());
		}
	}

	public class DeploymentCriterium
	{
		[HideInInspector]
		public int previousFieldID = -1;

		[ValueDropdown("GetActionFieldIdentifiers"), OnValueChanged("SetPreviousFieldID")]
		public string contextField;

		[ShowIf("@this.previousFieldID != -1"), HideReferenceObjectPicker]
		public IncidentCriteria criteria;

		public bool Evaluate(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			var fieldAction = delayedCalculateAction.Invoke(previousFieldID);
			if (fieldAction.GetFieldValue() == null)
			{
				if (fieldAction.CalculateField(context, delayedCalculateAction))
				{
					var contextProvider = fieldAction.GetFieldValue();
					return criteria.Evaluate(contextProvider.GetContext());
				}
			}
			else
			{
				var contextProvider = fieldAction.GetFieldValue();
				return criteria.Evaluate(contextProvider.GetContext());
			}
			return false;
		}

		private List<string> GetActionFieldIdentifiers()
		{
			var ids = new List<string>();
			var matches = IncidentEditorWindow.actionFields;
			matches.ForEach(x => ids.Add(x.NameID));
			return ids;
		}

		private void SetPreviousFieldID()
		{
			var actionField = IncidentEditorWindow.actionFields.Find(x => x.NameID == contextField);
			criteria = new IncidentCriteria(actionField.ContextType);
			previousFieldID = actionField.ActionFieldID;
		}
	}

	public interface IDeployableContext : IIncidentContext
	{
		bool CalculateFields(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction);
	}

	public abstract class DeployableContext : IDeployableContext
	{
		public IIncidentContextProvider Provider => null;

		public Type ContextType => this.GetType();

		[ShowInInspector]
		public int NumIncidents { get; set; }

		public int ParentID { get; set; }

		public bool CalculateFields(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			var fields = ContextType.GetFields();
			var matchingFields = fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(DeployedContextActionField<>)).ToList();

			foreach (var field in matchingFields)
			{
				var actionField = field.GetValue(this) as IIncidentActionField;
				if (!actionField.CalculateField(context, delayedCalculateAction))
				{
					return false;
				}
			}

			return true;
		}
	}

	public class WarDeclaredContext : DeployableContext
	{
		[HideReferenceObjectPicker]
		public DeployedContextActionField<FactionContext> faction1;
		[HideReferenceObjectPicker]
		public DeployedContextActionField<FactionContext> faction2;
	}
}