using System;

namespace Game.Incidents
{
	public class FloatEvaluator : CriteriaEvaluator<float>
    {
        public FloatEvaluator() : base() { }
        public FloatEvaluator(string propertyName) : base(propertyName) { }
        public FloatEvaluator(string propertyName, float value) : base(propertyName, value) { }
        public FloatEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }

        override public void Setup()
        {
            Comparators = ExpressionHelpers.FloatComparators;
            Operators = ExpressionHelpers.FloatOperators;
        }
    }
}