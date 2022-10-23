using System;
using System.Linq;

namespace Game.Incidents
{
	abstract public class IncidentAction<T> : IIncidentAction where T : IIncidentContext
	{
		private IIncidentContext context;
		public Type ContextType => typeof(T);
		public IIncidentContext Context
		{
			get { return context; }
			set
			{
				if (value != null)
				{
					if (value.ContextType == ContextType)
					{
						context = value;
					}
					else
					{
						OutputLogger.LogError(string.Format("Cannot assign context of type {0} to action with context type {1}", value.GetType().ToString(), ContextType.ToString()));
					}
				}
			}
		}

		public bool VerifyAction(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			return VerifyContextActionFields(context, delayedCalculateAction);
		}

		abstract public void PerformAction(IIncidentContext context);

		public void UpdateEditor()
		{
			var fields = this.GetType().GetFields();
			var matchingFields = fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(IncidentContextActionField<>)).ToList();

			foreach (var field in matchingFields)
			{
				field.SetValue(this, Activator.CreateInstance(field.FieldType, ContextType));
			}
		}
		protected bool VerifyContextActionFields(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			var fields = this.GetType().GetFields();
			var matchingFields = fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(IncidentContextActionField<>)).ToList();

			foreach (var field in matchingFields)
			{
				var actionField = field.GetValue(this) as IIncidentActionField;
				if(!actionField.CalculateField(context, delayedCalculateAction))
				{
					return false;
				}
			}

			return true;
		}
	}
}