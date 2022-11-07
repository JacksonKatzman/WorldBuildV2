using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Game.Incidents
{
	abstract public class IncidentAction : IIncidentAction
	{
		virtual public bool VerifyAction(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			var matchingFields = GetContexualActionFields();

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

		abstract public void PerformAction(IIncidentContext context, ref IncidentReport report);

		virtual public void UpdateEditor()
		{
			var matchingFields = GetContexualActionFields();

			foreach (var field in matchingFields)
			{
				field.SetValue(this, Activator.CreateInstance(field.FieldType, IncidentEditorWindow.ContextType));
			}

			matchingFields = GetIntegerRangeFields();

			foreach (var field in matchingFields)
			{
				field.SetValue(this, Activator.CreateInstance(field.FieldType));
			}
		}

		virtual public void UpdateActionFieldIDs(ref int startingValue)
		{
			var matchingFields = GetContexualActionFields();

			foreach (var f in matchingFields)
			{
				var fa = f.GetValue(this) as IIncidentActionField;
				fa.ActionFieldID = startingValue;
				fa.NameID = string.Format("{0}:{1}:{2}", fa.ActionFieldIDString, GetType().Name, f.Name);
				IncidentEditorWindow.actionFields.Add(fa);
				startingValue++;
			}
		}

		virtual public void AddContext(ref IncidentReport report)
		{
			var matchingFields = GetContexualActionFields();

			foreach (var field in matchingFields)
			{
				var actionField = field.GetValue(this) as IIncidentActionField;
				report.Contexts.Add(actionField.ActionFieldIDString, actionField.GetFieldValue());
			}
		}

		virtual public bool GetContextField(int id, out IIncidentActionField contextField)
		{
			var matchingFields = GetContexualActionFields();

			foreach (var field in matchingFields)
			{
				var actionField = field.GetValue(this) as IIncidentActionField;
				if (actionField.ActionFieldID == id)
				{
					contextField = actionField;
					return true;
				}
			}

			contextField = null;
			return false;
		}

		private IEnumerable<FieldInfo> GetContexualActionFields()
		{
			var fields = this.GetType().GetFields();
			return fields.Where(x => (x.FieldType.IsGenericType && (x.FieldType.GetGenericTypeDefinition() == typeof(ContextualIncidentActionField<>)
			|| x.FieldType.GetGenericTypeDefinition() == typeof(ActionResultField<>))) || x.FieldType == typeof(LocationActionField));
		}

		private IEnumerable<FieldInfo> GetIntegerRangeFields()
		{
			var fields = this.GetType().GetFields();
			return fields.Where(x => x.FieldType == typeof(IntegerRange));
		}
	}
}