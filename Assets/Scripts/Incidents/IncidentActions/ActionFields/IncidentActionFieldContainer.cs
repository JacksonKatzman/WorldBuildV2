using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	[HideReferenceObjectPicker]
	public class IncidentActionFieldContainer
	{
		[ValueDropdown("GetFilteredTypeList"), OnValueChanged("SetContextType"), LabelText("Context Type")]
		public Type contextType;
		[ShowIf("@this.actionField != null")]
		public IIncidentActionField actionField;

		[HideInInspector]
		public Action onSetContextType;

		private void SetContextType()
		{
			if (contextType == typeof(Location))
			{
				actionField = new LocationActionField();
			}
			else
			{
				var dataType = new Type[] { contextType };
				var genericBase = typeof(ContextualIncidentActionField<>);
				var combinedType = genericBase.MakeGenericType(dataType);
				actionField = (IIncidentActionField)Activator.CreateInstance(combinedType);
			}
			IncidentEditorWindow.UpdateActionFieldIDs();

			onSetContextType.Invoke();
		}
		virtual protected IEnumerable<Type> GetFilteredTypeList()
		{
			var q = typeof(IIncidentContext).Assembly.GetTypes()
				.Where(x => !x.IsAbstract)                                          // Excludes BaseClass
				.Where(x => !x.IsGenericTypeDefinition)                             // Excludes Generics
				.Where(x => typeof(IIncidentContext).IsAssignableFrom(x))           // Excludes classes not inheriting from IIncidentContext
				.Where(x => x.BaseType != typeof(SpecialFaction));                  // Excludes special factions for now

			return q;
		}
	}
}