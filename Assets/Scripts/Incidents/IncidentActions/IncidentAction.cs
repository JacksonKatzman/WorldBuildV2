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
			var fields = this.GetType().GetFields();
			var matchingFields = fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(ContextualIncidentActionField<>)).ToList();

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

		abstract public void PerformAction(IIncidentContext context);

		virtual public void UpdateEditor()
		{
			var matchingFields = GetContexualActionFields();

			foreach (var field in matchingFields)
			{
				field.SetValue(this, Activator.CreateInstance(field.FieldType));
			}
		}

		virtual public void UpdateActionFieldIDs(ref int startingValue)
		{
			IncidentEditorWindow.actionFields.Clear();

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

		virtual public void AddContext(ref Dictionary<string, IIncidentContext> contextDictionary)
		{
			var matchingFields = GetContexualActionFields();

			foreach (var field in matchingFields)
			{
				var actionField = field.GetValue(this) as IIncidentActionField;
				contextDictionary.Add(actionField.ActionFieldIDString, actionField.GetFieldValue());
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
			return fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(ContextualIncidentActionField<>));
		}
	}
}