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
            foreach(var modifier in modifiers)
			{
                modifier.Modify(contextToModify.GetTypedFieldValue());
			}
            OutputLogger.Log("Context Modified Via Action.");
		}

		public override void UpdateActionFieldIDs(ref int startingValue)
		{
			base.UpdateActionFieldIDs(ref startingValue);
			foreach(var mod in modifiers)
			{
				mod.Calculator.ID = startingValue;
				IncidentEditorWindow.calculators.Add(mod.Calculator);
				startingValue++;
			}
		}

		public override void UpdateEditor()
		{
			base.UpdateEditor();
			if (modifiers == null)
			{
				modifiers = new List<ContextModifier<T>>();
			}
		}

		private void AddNewModifier()
		{
            modifiers.Add(new ContextModifier<T>());
		}

		private void RemoveModifier(int index)
		{
			modifiers.RemoveAt(index);
			IncidentEditorWindow.UpdateActionFieldIDs();
		}
	}
}