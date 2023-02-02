using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class ActionFieldListContainsEvaluator : ActionFieldCriteriaEvaluator<List<IIncidentContext>, bool>
	{
		[ReadOnly]
		public string warning = "This evaluator finds items that exist within the chosen List within the parent ({0}) context.";
		public ActionFieldListContainsEvaluator() { }
		public ActionFieldListContainsEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }
		public override bool Evaluate(IIncidentContext context, string propertyName, IIncidentContext parentContext = null)
		{
			var propertyValue = (List<IIncidentContext>)parentContext.GetType().GetProperty(propertyName).GetValue(parentContext);
			return Comparators[Comparator].Invoke(propertyValue.Contains(context), true);
		}

		public override void Setup()
		{
			Comparators = ExpressionHelpers.BoolComparators;
		}
	}
}