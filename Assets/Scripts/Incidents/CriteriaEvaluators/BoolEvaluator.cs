using System;

namespace Game.Incidents
{
	public class BoolEvaluator : CriteriaEvaluator<bool>
    {
        public BoolEvaluator() : base() { }
        public BoolEvaluator(string propertyName) : base(propertyName) { }
        public BoolEvaluator(string propertyName, bool value) : base(propertyName, value) { }
        public BoolEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }

        override public void Setup()
        {
            Comparators = ExpressionHelpers.BoolComparators;
            Operators = ExpressionHelpers.BoolOperators;
        }
    }
}