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

        [ListDrawerSettings(CustomAddFunction = "AddNewModifier"), HorizontalGroup("Group 1"), HideReferenceObjectPicker]
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

		public override void UpdateEditor()
		{
			base.UpdateEditor();
            modifiers = new List<ContextModifier<T>>();
		}

		private void AddNewModifier()
		{
            modifiers.Add(new ContextModifier<T>());
		}
	}
}