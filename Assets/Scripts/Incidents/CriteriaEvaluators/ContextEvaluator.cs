using Sirenix.OdinInspector;
using System;

namespace Game.Incidents
{
	abstract public class ContextEvaluator<T, U> : ActionFieldCriteriaEvaluator<T, IIncidentContext> where T: IIncidentContext
	{
		public InterfacedIncidentActionFieldContainer<U> compareTo;

		public ContextEvaluator() : base() { }
		public ContextEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }

		override public bool Evaluate(IIncidentContext context, string propertyName, IIncidentContext parentContext)
		{
			compareTo.actionField.CalculateField(context);
			var parentValue = GetContext(compareTo.actionField.GetFieldValue());
			var contextValue = GetContext(context);

			return Comparators[Comparator].Invoke(parentValue, contextValue);
		}

		abstract protected T GetContext(IIncidentContext context);

		override public void Setup()
		{
			Comparators = ExpressionHelpers.ContextComparators;
			compareTo = new InterfacedIncidentActionFieldContainer<U>();
		}
	}
}