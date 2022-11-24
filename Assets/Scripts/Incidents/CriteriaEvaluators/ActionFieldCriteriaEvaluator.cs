using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
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
}