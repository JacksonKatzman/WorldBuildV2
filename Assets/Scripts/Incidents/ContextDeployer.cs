using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class ContextDeployer : IContextDeployer
	{
		public int delayTime = 0;

		[TypeFilter("GetFilteredTypeList"), OnValueChanged("SetContextType"), LabelText("Incident Type")]
		public IDeployableContext incidentContext;

		public void Deploy(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			incidentContext.CalculateFields(context, delayedCalculateAction);

			if(delayTime == 0)
			{
				IncidentService.Instance.PerformIncidents(incidentContext);
			}
			else
			{
				IncidentService.Instance.AddDelayedContext(incidentContext, delayTime);
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

		void SetContextType()
		{
			var fields = incidentContext.GetType().GetFields();
			var matchingFields = fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(DeployedContextActionField<>)).ToList();

			foreach (var field in matchingFields)
			{
				field.SetValue(incidentContext, Activator.CreateInstance(field.FieldType, this.GetType()));
			}
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