using Sirenix.OdinInspector;

namespace Game.Incidents
{
	abstract public class ContextEvaluator<T> : ActionFieldCriteriaEvaluator<T, IIncidentContext> where T: IIncidentContext
	{
		[HorizontalGroup("Group 1", 100), ReadOnly, HideLabel]
		public string toWho = "Mine";

		public ContextEvaluator() : base() { }

		override public bool Evaluate(IIncidentContext context, string propertyName, IIncidentContext parentContext)
		{
			var parentValue = GetContext(parentContext);
			var contextValue = GetContext(context);

			return Comparators[Comparator].Invoke(parentValue, contextValue);
		}

		abstract protected T GetContext(IIncidentContext context);

		override public void Setup()
		{
			Comparators = ExpressionHelpers.ContextComparators;
		}
	}
}