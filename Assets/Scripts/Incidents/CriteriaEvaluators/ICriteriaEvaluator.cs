using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public interface ICriteriaEvaluator
	{
        Type Type { get; }
        bool Evaluate(IIncidentContext context, string propertyName, IIncidentContext parentContext = null);
		void Setup();
	}

	abstract public class ContextEvaluator<T, V> : ICriteriaEvaluator where T : IIncidentContext where V : IIncidentContext
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

		public void Setup()
		{
			Comparators = ExpressionHelpers.ContextComparators;
		}

		private List<string> GetComparatorNames()
		{
			return Comparators.Keys.ToList();
		}
	}

	public class FactionEvaluator : ContextEvaluator<Faction, Person>
	{
		protected override Faction GetContext(IIncidentContext context)
		{
			if (context.ContextType == typeof(Faction))
			{
				return (Faction)context;
			}
			else if (context.ContextType == typeof(Person))
			{
				return ((Person)context).Faction;
			}
			else
			{
				return null;
			}
		}
	}
}