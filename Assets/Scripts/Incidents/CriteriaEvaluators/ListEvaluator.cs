using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class ListEvaluator : CriteriaEvaluator<int>
	{
        public ListEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }
        override public bool Evaluate(IIncidentContext context, string propertyName)
        {
            var propertyValue = (List<IIncidentContext>)context.GetType().GetProperty(propertyName).GetValue(context);

            return Comparators[Comparator].Invoke(propertyValue.Count, CombineExpressions(context));
        }
        protected override void Setup()
        {
            Comparators = ExpressionHelpers.IntegerComparators;
            Operators = ExpressionHelpers.IntegerOperators;
        }
    }
}