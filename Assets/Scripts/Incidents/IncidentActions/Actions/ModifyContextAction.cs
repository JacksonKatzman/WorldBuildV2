using Game.Debug;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	abstract public class ModifyContextAction<T> : GenericIncidentAction where T : IIncidentContext
	{
		public ContextualIncidentActionField<T> contextToModify;

        [ListDrawerSettings(CustomAddFunction = "AddNewModifier", CustomRemoveIndexFunction = "RemoveModifier"), HorizontalGroup("Group 1"), HideReferenceObjectPicker]
        public List<ContextModifier<T>> modifiers;

        public ModifyContextAction() { }

        override public void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			GameProfiler.BeginProfiling("ModifyCharacterAction", GameProfiler.ProfileFunctionType.DEPLOY);
			foreach (var modifier in modifiers)
			{
                modifier.Modify(contextToModify.GetTypedFieldValue());
			}
			GameProfiler.EndProfiling("ModifyCharacterAction");
			OutputLogger.Log("Context Modified Via Action.");
		}

		public override void UpdateActionFieldIDs(ref int startingValue)
		{
#if UNITY_EDITOR
			base.UpdateActionFieldIDs(ref startingValue);
			foreach(var mod in modifiers)
			{
				mod.Calculator.ID = startingValue;
				IncidentEditorWindow.calculators.Add(mod.Calculator);
				startingValue++;
			}
#endif
		}

		public override void UpdateEditor()
		{
			base.UpdateEditor();
			if (modifiers == null)
			{
				modifiers = new List<ContextModifier<T>>();
			}
		}
#if UNITY_EDITOR
		private void AddNewModifier()
		{
            modifiers.Add(new ContextModifier<T>());
		}

		private void RemoveModifier(int index)
		{
			modifiers.RemoveAt(index);
			IncidentEditorWindow.UpdateActionFieldIDs();
		}
#endif
	}
}