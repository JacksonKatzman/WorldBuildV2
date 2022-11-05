using System;

namespace Game.Incidents
{
	public class FloatValueDictionaryEvaluator : DictionaryEvaluator<float>
    {
        public FloatValueDictionaryEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }
        public override void Setup()
        {
            Comparators = ExpressionHelpers.FloatComparators;
            Operators = ExpressionHelpers.FloatOperators;
        }
    }
}