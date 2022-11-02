using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Game.Incidents
{
	abstract public class GenericIncidentAction : IncidentAction
	{
		
	}

	public class BranchingAction : GenericIncidentAction
	{
		[ListDrawerSettings(CustomAddFunction = "AddBranch"), HideReferenceObjectPicker]
		public List<IncidentActionBranch> branches;

		public BranchingAction()
		{
		}

		override public bool VerifyAction(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			foreach (var branch in branches)
			{
				if(!branch.VerifyActions(context, delayedCalculateAction))
				{
					return false;
				}
			}

			return true;
		}

		override public void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			//instead of foreach need to choose one based on weights
			foreach (var branch in branches)
			{
				branch.PerformActions(context, ref report);
			}
		}

		override public void UpdateEditor()
		{
			base.UpdateEditor();
			branches = new List<IncidentActionBranch>();
		}

		override public void UpdateActionFieldIDs(ref int startingValue)
		{
			foreach(var branch in branches)
			{
				branch.UpdateActionFieldIDs(ref startingValue);
			}
		}
/*
		override public void AddContext(ref IncidentReport report)
		{
			foreach (var branch in branches)
			{
				branch.AddContext(ref report);
			}
		}
*/
		override public bool GetContextField(int id, out IIncidentActionField contextField)
		{
			foreach (var branch in branches)
			{
				var test = branch.GetContextField(id);
				if(test != null)
				{
					contextField = test;
					return true;
				}
			}
			contextField = null;
			return false;
		}

		private void AddBranch()
		{
			branches.Add(new IncidentActionBranch(IncidentEditorWindow.ContextType));
		}
	}

	public class IncidentActionBranch
	{
		public int weight;

		[HideReferenceObjectPicker]
		public IncidentActionHandler actionHandler;

		public IncidentActionBranch(Type type)
		{
			actionHandler = new IncidentActionHandler(type);
		}

		public bool VerifyActions(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			return actionHandler.VerifyActions(context, delayedCalculateAction);
		}

		public void PerformActions(IIncidentContext context, ref IncidentReport report)
		{
			actionHandler.PerformActions(context, ref report);
		}

		public void UpdateActionFieldIDs(ref int startingValue)
		{
			actionHandler.UpdateActionFieldIDs(ref startingValue);
		}

		public void AddContext(ref IncidentReport report)
		{
			actionHandler.GetContextDictionary(ref report);
		}

		public IIncidentActionField GetContextField(int id)
		{
			return actionHandler.GetContextFromActionFields(id);
		}
	}

	static public class IncidentActionHelpers
	{
		static public IEnumerable<Type> GetAllTypesImplementingOpenGenericType(Type openGenericType, Assembly assembly)
		{
			return from x in assembly.GetTypes()
				   from z in x.GetInterfaces()
				   let y = x.BaseType
				   where
				   (y != null && y.IsGenericType &&
				   openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition())) ||
				   (z.IsGenericType &&
				   openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
				   select x;
		}

		static public IEnumerable<Type> GetAllTypesImplementingType(Type type, Assembly assembly)
		{
			return assembly
						.GetTypes()
						.Where(t => type.IsAssignableFrom(t) &&
									t != type);
		}

		static public IEnumerable<Type> GetFilteredTypeList(Type contextType)
		{
			var allActionsWithGenericParents = GetAllTypesImplementingOpenGenericType(typeof(IIncidentAction), Assembly.GetExecutingAssembly()).ToList();
			var contextualActions = allActionsWithGenericParents.Where(x => (x.BaseType.IsGenericType == true && x.BaseType.GetGenericArguments()[0] == contextType));
			var genericActions = GetAllTypesImplementingType(typeof(GenericIncidentAction), Assembly.GetExecutingAssembly());
			var matches = contextualActions.Concat(genericActions).ToList();
			return matches;
		}
	}
}