using Game.Factions;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Data;
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

		public void Deploy(IIncidentContext context)
		{
			if(incidentContext == null)
			{
				return;
			}

			foreach(var criterium in deploymentCriteria)
			{
				if(!criterium.Evaluate(context))
				{
					OutputLogger.Log("Failed deployment criteria!");
					return;
				}
			}

			incidentContext.CalculateFields(context);

			if(delayTime == 0)
			{
				IncidentService.Instance.AddFollowUpContext(incidentContext);
			}
			else
			{
				IncidentService.Instance.AddDelayedContext(incidentContext, delayTime);
			}

			OutputLogger.Log("Deployed Criteria!");
		}

		public void UpdateContextIDs(Dictionary<int, IIncidentActionField> updates)
		{
			var fields = incidentContext.GetType().GetFields();
			var matchingFields = fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(DeployedContextActionField<>)).ToList();

			foreach (var matchingField in matchingFields)
			{
				var field = matchingField.GetValue(incidentContext) as IIncidentActionField;
				var prev = field.PreviousFieldID;
				if (updates.ContainsKey(prev))
				{
					field.PreviousFieldID = updates[prev].ActionFieldID;
					field.PreviousField = updates[prev].NameID;
				}
			}
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

		public bool Evaluate(IIncidentContext context)
		{
			var fieldAction = IncidentService.Instance.CurrentIncident.ActionContainer.GetContextFromActionFields(previousFieldID);
			if (fieldAction.GetFieldValue() == null)
			{
				if (fieldAction.CalculateField(context))
				{
					var calculatedContext = fieldAction.GetFieldValue();
					return criteria.Evaluate(calculatedContext);
				}
			}
			else
			{
				var calculatedContext = fieldAction.GetFieldValue();
				return criteria.Evaluate(calculatedContext);
			}
			return false;
		}
#if UNITY_EDITOR
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
#endif
	}
	public interface IDeployableContext : IIncidentContext
	{
		bool CalculateFields(IIncidentContext context);
	}

	public abstract class DeployableContext : IDeployableContext
	{
		public Type ContextType => this.GetType();

		[ShowInInspector]
		public int NumIncidents { get; set; }
		public int ID { get; set; }

		public int ParentID { get; set; }
		public string Name { get; set; }

		public bool CalculateFields(IIncidentContext context)
		{
			var fields = GetType().GetFields();
			var matchingFields = fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(DeployedContextActionField<>)).ToList();

			foreach (var field in matchingFields)
			{
				var actionField = field.GetValue(this) as IIncidentActionField;
				if (!actionField.CalculateField(context))
				{
					return false;
				}
			}

			return true;
		}

		public void DeployContext() { }

		public void Die()
		{
		}

		public DataTable GetDataTable()
		{
			return null;
		}

		public void UpdateContext() { }

		public void UpdateHistoricalData()
		{
		}

		public void LoadContextProperties()
		{
		}
	}
}