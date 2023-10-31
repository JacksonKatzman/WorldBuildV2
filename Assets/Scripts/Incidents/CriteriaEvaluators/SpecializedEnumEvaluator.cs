using Sirenix.OdinInspector;
using System;

namespace Game.Incidents
{
	abstract public class SpecializedEnumEvaluator<T, U> : ActionFieldCriteriaEvaluator<T, Enum> where T : Enum
	{
		public bool specialized;
		[ShowIf("@!this.specialized")]
		public EnumEvaluator<T> standardEvaluator;
		[ShowIf("@this.specialized")]
		public InterfacedIncidentActionFieldContainer<U> compareTo;

		public SpecializedEnumEvaluator() : base() { }
		public SpecializedEnumEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }

		override public bool Evaluate(IIncidentContext context, string propertyName, IIncidentContext parentContext)
		{
			if (specialized)
			{
				compareTo.actionField.CalculateField(context);
				var parentValue = GetEnumValue(compareTo.actionField.GetFieldValue());
				var contextValue = GetEnumValue(context);

				if (parentValue == null || contextValue == null)
				{
					return false;
				}

				return Comparators[Comparator].Invoke(parentValue, contextValue);
			}
			else
			{
				return standardEvaluator.Evaluate(context, propertyName, parentContext);
			}
		}

		abstract protected T GetEnumValue(IIncidentContext context);

		override public void Setup()
		{
			Comparators = ExpressionHelpers.EnumComparators;
			standardEvaluator = new EnumEvaluator<T>(propertyName, ContextType);
			compareTo = new InterfacedIncidentActionFieldContainer<U>();
		}
	}
}