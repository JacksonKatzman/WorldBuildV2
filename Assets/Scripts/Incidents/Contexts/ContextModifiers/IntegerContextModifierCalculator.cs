using System;

namespace Game.Incidents
{
	public class IntegerContextModifierCalculator : ContextModifierCalculator<int>
    {
        public IntegerContextModifierCalculator(string propertyName, Type contextType) : base(propertyName, contextType) { }
        public override void Setup()
        {
            Operators = ExpressionHelpers.IntegerOperators;
        }
    }
}