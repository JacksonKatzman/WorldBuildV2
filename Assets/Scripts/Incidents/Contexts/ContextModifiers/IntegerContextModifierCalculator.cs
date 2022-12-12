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

		protected override int Clamp(int value)
		{
			return value < 0 ? 0 : value;
		}
	}
}