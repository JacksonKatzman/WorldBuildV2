using System;

namespace Game.Incidents
{
	public class BoolValueDictionaryEvaluator : DictionaryEvaluator<bool>
    {
        public BoolValueDictionaryEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }
        public override void Setup()
        {
            Comparators = ExpressionHelpers.BoolComparators;
            Operators = ExpressionHelpers.BoolOperators;
        }
    }
}