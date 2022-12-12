using System;

namespace Game.Incidents
{
	public class BooleanContextModifierCalculator : ContextModifierCalculator<bool>
    {
        public BooleanContextModifierCalculator(string propertyName, Type contextType) : base(propertyName, contextType) { }
        public override void Setup()
        {
            Operators = ExpressionHelpers.BoolOperators;
        }

		protected override bool Clamp(bool value)
		{
			return value;
		}
	}
}