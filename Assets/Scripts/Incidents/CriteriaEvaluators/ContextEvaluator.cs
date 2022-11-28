using Sirenix.OdinInspector;

namespace Game.Incidents
{
	abstract public class ContextEvaluator<T> : ActionFieldCriteriaEvaluator<T, IIncidentContext> where T: IIncidentContext
	{
		public PreviousOnlyContextualIncidentActionField<T> compareTo;

		public ContextEvaluator() : base() { }

		override public bool Evaluate(IIncidentContext context, string propertyName, IIncidentContext parentContext)
		{
			compareTo.CalculateField(context);
			var parentValue = compareTo.GetTypedFieldValue();
			var contextValue = GetContext(context);

			return Comparators[Comparator].Invoke(parentValue, contextValue);
		}

		abstract protected T GetContext(IIncidentContext context);

		override public void Setup()
		{
			Comparators = ExpressionHelpers.ContextComparators;
			compareTo = new PreviousOnlyContextualIncidentActionField<T>();
		}
	}
}