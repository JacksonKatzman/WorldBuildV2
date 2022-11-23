using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	abstract public class ContextEvaluator<T> : ActionFieldCriteriaEvaluator<T, IIncidentContext> where T: IIncidentContext
	{
		[HorizontalGroup("Group 1", 100), ReadOnly, HideLabel]
		public string toWho = "Mine";

		public ContextEvaluator() : base() { }

		override public bool Evaluate(IIncidentContext context, string propertyName, IIncidentContext parentContext)
		{
			var parentValue = GetContext(parentContext);
			var contextValue = GetContext(context);

			return Comparators[Comparator].Invoke(parentValue, contextValue);
		}

		abstract protected T GetContext(IIncidentContext context);

		override public void Setup()
		{
			Comparators = ExpressionHelpers.ContextComparators;
		}
	}

	abstract public class ActionFieldCriteriaEvaluator<T, V> : ICriteriaEvaluator
	{
		//Next step would be to make this inherit from CriteriaEvaluator not just the interface
		protected Dictionary<string, Func<V, V,bool>> Comparators { get; set; }

		public Type Type => typeof(T);
		public Type ContextType { get; set; }

		[HorizontalGroup("Group 1", 150), HideLabel, ReadOnly]
		public string propertyName;

		[ValueDropdown("GetComparatorNames"), HorizontalGroup("Group 1", 50), HideLabel]
		public string Comparator;

		public ActionFieldCriteriaEvaluator()
		{
			Setup();
		}

		public ActionFieldCriteriaEvaluator(string propertyName, Type contextType) : this()
		{
			this.propertyName = propertyName;
			ContextType = contextType;
		}

		abstract public bool Evaluate(IIncidentContext context, string propertyName, IIncidentContext parentContext = null);

		abstract public void Setup();

		private List<string> GetComparatorNames()
		{
			return Comparators.Keys.ToList();
		}
	}

	public class ActionFieldIntDictionaryEvaluator : ActionFieldCriteriaEvaluator<Dictionary<IIncidentContext, int>, int>
	{
		[HorizontalGroup("Group 1")]
		public int value;
		public ActionFieldIntDictionaryEvaluator() { }
		public ActionFieldIntDictionaryEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }
		public override bool Evaluate(IIncidentContext context, string propertyName, IIncidentContext parentContext = null)
		{
			var propertyValue = (Dictionary<IIncidentContext, int>)context.GetType().GetProperty(propertyName).GetValue(context);
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