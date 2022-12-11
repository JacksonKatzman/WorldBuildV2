using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class IncidentActionBranchWeightModifier
	{
		[OnValueChanged("ToggleAdvancedMode")]
		public bool advancedMode;
		[ShowIf("@this.advancedMode == false")]
		public int baseWeight;
		[ShowIf("@this.advancedMode == true")]
		public IncidentActionFieldContainer container;
		[ShowIf("ShowWeight"), HideReferenceObjectPicker]
		public IIncidentWeight weight;
		private bool ShowWeight => advancedMode && weight != null;
		public IncidentActionBranchWeightModifier()
		{
			
		}
		public IncidentActionBranchWeightModifier(Type contextType)
		{
			container = new IncidentActionFieldContainer();
			container.onSetContextType += Setup;
		}

		public bool VerifyField(IIncidentContext context)
		{
			return !advancedMode || container.actionField.CalculateField(context);
		}

		public int Calculate()
		{
			return advancedMode ? weight.CalculateWeight(container.actionField.GetFieldValue()) : baseWeight;
		}

		private void ToggleAdvancedMode()
		{
			if(advancedMode && container == null)
			{
				container = new IncidentActionFieldContainer();
			}

			container.onSetContextType += Setup;
		}

		private void Setup()
		{
			var dataType = new Type[] { container.contextType };
			var genericBase = typeof(IncidentWeight<>);
			var combinedType = genericBase.MakeGenericType(dataType);
			weight = (IIncidentWeight)Activator.CreateInstance(combinedType);
		}

		//Step 1: Have a ContextualIncidentActionField where we can first choose the context type (like in the get contexts action)
		//Step 2: Once we know the type, use it to create an IncidentModifier<T>
		//Step 3: As part of the branch weighting, we first grab the context from the action field and pass that into the modifiers calculator
	}
}