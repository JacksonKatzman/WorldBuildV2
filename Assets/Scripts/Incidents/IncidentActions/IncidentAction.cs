using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Game.Incidents
{
	abstract public class IncidentAction : IIncidentAction
	{
		virtual public bool VerifyAction(IIncidentContext context)
		{
			var matchingFields = GetContexualActionFields();
			var matchingLists = GetCollectionsOfActionFields();
			var matchingContainers = GetActionFieldContainers();

			foreach (var field in matchingFields)
			{
				var actionField = field.GetValue(this) as IIncidentActionField;
				if (!actionField.CalculateField(context))
				{
					OutputLogger.Log(String.Format("{0} failed to verify.", GetType().Name));
					return false;
				}
			}

			foreach (var l in matchingLists)
			{
				var list = l.GetValue(this) as List<IncidentActionFieldContainer>;
				foreach (var actionFieldContainer in list)
				{
					if (!actionFieldContainer.actionField.CalculateField(context))
					{
						return false;
					}
				}
			}

			foreach(var c in matchingContainers)
			{
				var container = c.GetValue(this) as IncidentActionFieldContainer;
				if(!container.actionField.CalculateField(context))
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

			matchingFields = GetCollectionsOfActionFields();

			foreach(var field in matchingFields)
			{
				field.SetValue(this, Activator.CreateInstance(field.FieldType));
			}

			matchingFields = GetActionFieldContainers();

			foreach (var field in matchingFields)
			{
				field.SetValue(this, Activator.CreateInstance(field.FieldType));
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
			var matchingLists = GetCollectionsOfActionFields();
			var matchingContainers = GetActionFieldContainers();

			foreach (var f in matchingFields)
			{
				var fa = f.GetValue(this) as IIncidentActionField;
				fa.ActionFieldID = startingValue;
				fa.NameID = string.Format("{0}:{1}:{2}", fa.ActionFieldIDString, GetType().Name, f.Name);
				IncidentEditorWindow.actionFields.Add(fa);
				startingValue++;
			}

			foreach(var l in matchingLists)
			{
				var list = l.GetValue(this) as List<IncidentActionFieldContainer>;
				foreach(var f in list)
				{
					f.actionField.ActionFieldID = startingValue;
					f.actionField.NameID = string.Format("{0}:{1}:{2}", f.actionField.ActionFieldIDString, GetType().Name, l.Name);
					IncidentEditorWindow.actionFields.Add(f.actionField);
					startingValue++;
				}
			}

			foreach (var c in matchingContainers)
			{
				var container = c.GetValue(this) as IncidentActionFieldContainer;
				if (container.contextType != null)
				{
					var fa = container.actionField;
					fa.ActionFieldID = startingValue;
					fa.NameID = string.Format("{0}:{1}:{2}", fa.ActionFieldIDString, GetType().Name, c.Name);
					IncidentEditorWindow.actionFields.Add(fa);
					startingValue++;
				}
			}
		}

		virtual public void AddContext(ref IncidentReport report)
		{
			var matchingFields = GetContexualActionFields();
			var matchingLists = GetCollectionsOfActionFields();
			var matchingContainers = GetActionFieldContainers();

			foreach (var field in matchingFields)
			{
				var actionField = field.GetValue(this) as IIncidentActionField;
				report.Contexts.Add(actionField.ActionFieldIDString, actionField.GetFieldValue());
			}

			foreach (var l in matchingLists)
			{
				var list = l.GetValue(this) as List<IncidentActionFieldContainer>;
				foreach (var actionFieldContainer in list)
				{
					var actionField = actionFieldContainer.actionField;
					report.Contexts.Add(actionField.ActionFieldIDString, actionField.GetFieldValue());
				}
			}

			foreach(var c in matchingContainers)
			{
				var container = c.GetValue(this) as IncidentActionFieldContainer;
				report.Contexts.Add(container.actionField.ActionFieldIDString, container.actionField.GetFieldValue());
			}
		}

		virtual public bool GetContextField(int id, out IIncidentActionField contextField)
		{
			var matchingFields = GetContexualActionFields();
			var matchingLists = GetCollectionsOfActionFields();
			var matchingContainers = GetActionFieldContainers();

			foreach (var field in matchingFields)
			{
				var actionField = field.GetValue(this) as IIncidentActionField;
				if (actionField.ActionFieldID == id)
				{
					contextField = actionField;
					return true;
				}
			}

			foreach (var l in matchingLists)
			{
				var list = l.GetValue(this) as List<IncidentActionFieldContainer>;
				foreach (var actionFieldContainer in list)
				{
					var actionField = actionFieldContainer.actionField;
					if (actionField.ActionFieldID == id)
					{
						contextField = actionField;
						return true;
					}
				}
			}

			foreach (var c in matchingContainers)
			{
				var container = c.GetValue(this) as IncidentActionFieldContainer;
				if (container.actionField.ActionFieldID == id)
				{
					contextField = container.actionField;
					return true;
				}
			}

			contextField = null;
			return false;
		}

		private IEnumerable<FieldInfo> GetContexualActionFields()
		{
			return ActionFieldReflection.GetGenericFieldsByType(this.GetType(),
				typeof(ContextualIncidentActionField<>),
				typeof(LocationActionField));

			/*
			var fields = this.GetType().GetFields();
			return fields.Where(x => (x.FieldType.IsGenericType && (x.FieldType.GetGenericTypeDefinition() == typeof(ContextualIncidentActionField<>)
			|| x.FieldType.GetGenericTypeDefinition() == typeof(ActionResultField<>))) || x.FieldType == typeof(LocationActionField));
			*/
		}

		private IEnumerable<FieldInfo> GetCollectionsOfActionFields()
		{
			return ActionFieldReflection.GetListsByType(this.GetType(), typeof(IncidentActionFieldContainer));
			/*
			var fields = this.GetType().GetFields();
			var lists = fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(List<>) && x.FieldType.GetGenericArguments()[0] == typeof(IncidentActionFieldContainer));
			return lists;
			*/
		}

		private IEnumerable<FieldInfo> GetActionFieldContainers()
		{
			return ActionFieldReflection.GetGenericFieldsByType(this.GetType(), typeof(InterfacedIncidentActionFieldContainer<>));

			//var fields = this.GetType().GetFields();
			//return fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(InterfacedIncidentActionFieldContainer<>));
		}

		private IEnumerable<FieldInfo> GetIntegerRangeFields()
		{
			return ActionFieldReflection.GetFieldsByType(this.GetType(), typeof(IntegerRange));
			//var fields = this.GetType().GetFields();
			//return fields.Where(x => x.FieldType == typeof(IntegerRange));
		}
	}

	public static class ActionFieldReflection
	{
		public static IEnumerable<FieldInfo> GetGenericFieldsByType(Type contextType, params Type[] types)
		{
			var fields = contextType.GetFields();
			var actionFields = new List<FieldInfo>();

			foreach(var type in types)
			{
				actionFields.AddRange(fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == type));
			}

			return actionFields;
			//return fields.Where(x => (x.FieldType.IsGenericType && (x.FieldType.GetGenericTypeDefinition() == typeof(ContextualIncidentActionField<>)
			//|| x.FieldType.GetGenericTypeDefinition() == typeof(ActionResultField<>))) || x.FieldType == typeof(LocationActionField));
		}

		public static IEnumerable<FieldInfo> GetListsByType(Type contextType, params Type[] types)
		{
			var fields = contextType.GetFields();
			var actionFields = new List<FieldInfo>();

			foreach (var type in types)
			{
				actionFields.AddRange(fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(List<>) && x.FieldType.GetGenericArguments()[0] == type));
			}

			return actionFields;

			//var lists = fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(List<>) && x.FieldType.GetGenericArguments()[0] == typeof(IncidentActionFieldContainer));
			//return lists;
		}

		public static IEnumerable<FieldInfo> GetFieldsByType(Type contextType, params Type[] types)
		{
			var fields = contextType.GetFields();
			var actionFields = new List<FieldInfo>();

			foreach (var type in types)
			{
				actionFields.AddRange(fields.Where(x => x.FieldType == type));
			}

			return actionFields;
			//return fields.Where(x => x.FieldType == typeof(IntegerRange));
		}
	}
}