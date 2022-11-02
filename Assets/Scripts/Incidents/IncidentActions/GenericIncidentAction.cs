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
		public override void PerformAction(IIncidentContext context)
		{
			throw new NotImplementedException();
		}

		public override void UpdateEditor()
		{
			base.UpdateEditor();
			branches = new List<IncidentActionBranch>();
		}

		private void AddBranch()
		{
			branches.Add(new IncidentActionBranch());
		}
	}

	public class IncidentActionBranch
	{
		public int weight;
		[TypeFilter("GetFilteredTypeList"), OnValueChanged("SetAction"), HideLabel]
		public List<IIncidentAction> actions;

		private void SetAction()
		{
			//incidentAction.UpdateEditor();
			//onSetCallback?.Invoke();
		}

		private IEnumerable<Type> GetFilteredTypeList()
		{
			return IncidentActionHelpers.GetFilteredTypeList(IncidentEditorWindow.ContextType);
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