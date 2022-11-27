using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class ActionFieldIntDictionaryEvaluator : ActionFieldCriteriaEvaluator<Dictionary<IIncidentContext, int>, int>
	{
		[HorizontalGroup("Group 1")]
		public int value;
		public ActionFieldIntDictionaryEvaluator() { }
		public ActionFieldIntDictionaryEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }
		public override bool Evaluate(IIncidentContext context, string propertyName, IIncidentContext parentContext = null)
		{
			var propertyValue = (Dictionary<IIncidentContext, int>)parentContext.GetType().GetProperty(propertyName).GetValue(parentContext);
			if(propertyValue.ContainsKey(context))
			{
				return Comparators[Comparator].Invoke(propertyValue[context], value);
			}
			return false;
		}

		public override void Setup()
		{
			Comparators = ExpressionHelpers.IntegerComparators;
		}
	}
}