using System;

namespace Game.Incidents
{
	public class IntegerValueDictionaryEvaluator : DictionaryEvaluator<int>
	{
        public IntegerValueDictionaryEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }
        override public void Setup()
		{
            Comparators = ExpressionHelpers.IntegerComparators;
            Operators = ExpressionHelpers.IntegerOperators;
        }
	}
}