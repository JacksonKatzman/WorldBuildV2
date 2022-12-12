using System;

namespace Game.Incidents
{
	public class FloatContextModifierCalculator : ContextModifierCalculator<float>
    {
        public FloatContextModifierCalculator(string propertyName, Type contextType) : base(propertyName, contextType) { }
        public override void Setup()
        {
            Operators = ExpressionHelpers.FloatOperators;
        }

		protected override float Clamp(float value)
		{
			return value < 0 ? 0 : value;
		}
	}
}