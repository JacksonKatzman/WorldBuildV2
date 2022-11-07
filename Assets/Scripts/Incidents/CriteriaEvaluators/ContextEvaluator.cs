using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	abstract public class ContextEvaluator<T> : ICriteriaEvaluator where T : IIncidentContext
	{
		protected Dictionary<string, Func<IIncidentContext, IIncidentContext, bool>> Comparators { get; set; }

		[ValueDropdown("GetComparatorNames"), HorizontalGroup("Group 1", 50), HideLabel]
		public string Comparator;

		[HorizontalGroup("Group 1", 100), ReadOnly, HideLabel]
		public string toWho = "Mine";

		public Type Type => typeof(T);

		public ContextEvaluator()
		{
			Setup();
		}

		public bool Evaluate(IIncidentContext context, string propertyName, IIncidentContext parentContext)
		{
			var parentValue = GetContext(parentContext);
			var contextValue = GetContext(context);

			return Comparators[Comparator].Invoke(parentValue, contextValue);
		}

		abstract protected T GetContext(IIncidentContext context);

		virtual public void Setup()
		{
			Comparators = ExpressionHelpers.ContextComparators;
		}

		private List<string> GetComparatorNames()
		{
			return Comparators.Keys.ToList();
		}
	}
}