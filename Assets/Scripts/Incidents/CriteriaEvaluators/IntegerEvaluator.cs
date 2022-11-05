using System;

namespace Game.Incidents
{
	public class IntegerEvaluator : CriteriaEvaluator<int>
	{
        public IntegerEvaluator() : base() { }
        public IntegerEvaluator(string propertyName) : base(propertyName) { }
        public IntegerEvaluator(string propertyName, int value) : base(propertyName, value) { }
        public IntegerEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }

        override protected void Setup()
		{
            Comparators = ExpressionHelpers.IntegerComparators;
            Operators = ExpressionHelpers.IntegerOperators;
		}
	}
}