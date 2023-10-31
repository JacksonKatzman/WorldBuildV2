using Game.Data;
using Game.Debug;
using Game.Simulation;
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
				var af = field.GetValue(this) as IIncidentActionField;
				if (!af.CalculateField(context))
				{
					OutputLogger.LogWarning(String.Format("{0} failed to verify in year {1} for incident: {2}.", GetType().Name, SimulationManager.Instance.world.Age, IncidentService.Instance.CurrentIncident.IncidentName));
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
				if(container.enabled && !container.actionField.CalculateField(context))
				{
					return false;
				}
			}

			return true;
		}

		abstract public void PerformAction(IIncidentContext context, ref IncidentReport report);

		virtual public void UpdateEditor()
		{
#if UNITY_EDITOR
			var matchingFields = GetContexualActionFields();

			foreach (var field in matchingFields)
			{
				if (field.GetValue(this) == null)
				{
					field.SetValue(this, Activator.CreateInstance(field.FieldType, IncidentEditorWindow.ContextType));
				}
			}

			matchingFields = GetCollectionsOfActionFields();

			foreach(var field in matchingFields)
			{
				if (field.GetValue(this) == null)
				{
					field.SetValue(this, Activator.CreateInstance(field.FieldType));
				}
			}

			matchingFields = GetActionFieldContainers();

			foreach (var field in matchingFields)
			{
				if (field.GetValue(this) == null)
				{
					field.SetValue(this, Activator.CreateInstance(field.FieldType));
				}
			}

			matchingFields = GetIntegerRangeFields();

			foreach (var field in matchingFields)
			{
				if (field.GetValue(this) == null)
				{
					field.SetValue(this, Activator.CreateInstance(field.FieldType));
				}
			}
#endif
		}

		virtual public void UpdateActionFieldIDs(ref int startingValue)
		{
#if UNITY_EDITOR
			UpdateEditor();

			var matchingFields = GetContexualActionFields();
			var matchingLists = GetCollectionsOfActionFields();
			var matchingContainers = GetActionFieldContainers();

			foreach (var f in matchingFields)
			{
				var fa = f.GetValue(this) as IIncidentActionField;
				if(fa.ActionFieldID != startingValue)
				{
					IncidentEditorWindow.UpdateLogIDs(fa.ActionFieldID, fa);
				}
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
					if (f.actionField.ActionFieldID != startingValue)
					{
						IncidentEditorWindow.UpdateLogIDs(f.actionField.ActionFieldID, f.actionField);
					}
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
					if (fa.ActionFieldID != startingValue)
					{
						IncidentEditorWindow.UpdateLogIDs(fa.ActionFieldID, fa);
					}
					fa.ActionFieldID = startingValue;
					fa.NameID = string.Format("{0}:{1}:{2}", fa.ActionFieldIDString, GetType().Name, c.Name);
					IncidentEditorWindow.actionFields.Add(fa);
					startingValue++;
				}
			}
#endif
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
				if (container.enabled)
				{
					report.Contexts.Add(container.actionField.ActionFieldIDString, container.actionField.GetFieldValue());
				}
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
				if (actionField != null && actionField.ActionFieldID == id)
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
				if (container.enabled && container.actionField.ActionFieldID == id)
				{
					contextField = container.actionField;
					return true;
				}
			}

			contextField = null;
			return false;
		}

		public List<int> GetAllActionFieldIDs()
		{
			var matchingFields = GetContexualActionFields();
			var matchingLists = GetCollectionsOfActionFields();
			var matchingContainers = GetActionFieldContainers();

			var ids = new List<int>();

			foreach (var f in matchingFields)
			{
				var fa = f.GetValue(this) as IIncidentActionField;
				ids.Add(fa.ActionFieldID);
			}

			foreach (var l in matchingLists)
			{
				var list = l.GetValue(this) as List<IncidentActionFieldContainer>;
				foreach (var f in list)
				{
					ids.Add(f.actionField.ActionFieldID);
				}
			}

			foreach (var c in matchingContainers)
			{
				var container = c.GetValue(this) as IncidentActionFieldContainer;
				if (container.contextType != null)
				{
					var fa = container.actionField;
					ids.Add(fa.ActionFieldID);
				}
			}

			return ids;
		}

		private IEnumerable<FieldInfo> GetContexualActionFields()
		{
			return ActionFieldReflection.GetGenericFieldsByType(this.GetType(),
				typeof(ContextualIncidentActionField<>),
				typeof(LocationActionField));
		}

		private IEnumerable<FieldInfo> GetCollectionsOfActionFields()
		{
			return ActionFieldReflection.GetListsByType(this.GetType(), typeof(IncidentActionFieldContainer));
		}

		private IEnumerable<FieldInfo> GetActionFieldContainers()
		{
			return ActionFieldReflection.GetGenericFieldsByType(this.GetType(), typeof(InterfacedIncidentActionFieldContainer<>));
		}

		private IEnumerable<FieldInfo> GetIntegerRangeFields()
		{
			return ActionFieldReflection.GetFieldsByType(this.GetType(), typeof(IntegerRange));
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
				if (type.IsGenericType)
				{
					actionFields.AddRange(fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == type));
				}
				else
				{
					actionFields.AddRange(fields.Where(x => x.FieldType == type));
				}
			}

			return actionFields;
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
		}
	}
}