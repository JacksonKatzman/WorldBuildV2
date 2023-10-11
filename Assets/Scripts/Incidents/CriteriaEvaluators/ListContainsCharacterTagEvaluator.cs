using Game.Enums;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class ListContainsCharacterTagEvaluator : CriteriaEvaluator<bool>
	{
        public CharacterTag tag;
        public ListContainsCharacterTagEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }

		protected override bool Evaluate(IIncidentContext context, string propertyName)
		{
            var propertyValue = (List<CharacterTag>)context.GetType().GetProperty(propertyName).GetValue(context);

            return Comparators[Comparator].Invoke(propertyValue.Contains(tag), CombineExpressions(context));
        }

		public override void Setup()
		{
            Comparators = ExpressionHelpers.BoolComparators;
            Operators = ExpressionHelpers.BoolOperators;
        }
	}
}