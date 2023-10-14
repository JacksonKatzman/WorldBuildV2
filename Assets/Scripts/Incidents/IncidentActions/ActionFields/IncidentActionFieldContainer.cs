﻿using Newtonsoft.Json;
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
		[ValueDropdown("GetFilteredTypeList"), OnValueChanged("SetContextType"), LabelText("Context Type"), ShowIf("@this.showTypeDropdown == true")]
		public Type contextType;

		[ShowIf("@this.showTypeDropdown != true"), ReadOnly]
		public string fieldName;

		[ShowIf("@this.actionField != null")]
		public IIncidentActionField actionField;

		[HideInInspector, JsonIgnore]
		public Action onSetContextType;

		[HideInInspector]
		public bool enabled = true;

		private bool showTypeDropdown = true;

		public void ForceSetContextType(Type type, string fieldName = "")
		{
			contextType = type;
			this.fieldName = fieldName;
			showTypeDropdown = false;
			SetContextType();
		}

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
#if UNITY_EDITOR
			IncidentEditorWindow.UpdateActionFieldIDs();
#endif

			onSetContextType?.Invoke();
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