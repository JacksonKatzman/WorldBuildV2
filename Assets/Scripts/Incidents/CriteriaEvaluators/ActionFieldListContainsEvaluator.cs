using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
			var propertyType = parentContext.GetType().GetProperty(propertyName).GetValue(parentContext).GetType().GetGenericArguments()[0];
			Type contextListType = typeof(List<>).MakeGenericType(propertyType);
			IList objectList = (IList)Activator.CreateInstance(contextListType);

			var propertyInfo = parentContext.GetType().GetProperty(propertyName);
			IEnumerable copyFrom = (IEnumerable)propertyInfo.GetValue(parentContext, null);
			foreach(var item in copyFrom)
			{
				objectList.Add(item);
			}
			return Comparators[Comparator].Invoke(objectList.Contains(context), true);
		}

		public override void Setup()
		{
			Comparators = ExpressionHelpers.BoolComparators;
		}
	}
}