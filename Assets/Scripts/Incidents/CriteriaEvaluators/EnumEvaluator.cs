using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class EnumEvaluator<T> : ICriteriaEvaluator
	{
		public Type Type => typeof(T);
		public Type ContextType { get; set; }

		[HorizontalGroup("Group 1", 150), HideLabel, ReadOnly]
		public string propertyName;

		[ValueDropdown("GetPossibleValues", IsUniqueList = true, DropdownTitle = "Allowed Values")]
		public List<T> allowedValues;

		public EnumEvaluator(string propertyName, Type contextType)
		{
			this.propertyName = propertyName;
			ContextType = contextType;
			Setup();
		}

		public bool Evaluate(IIncidentContext context, string propertyName, IIncidentContext parentContext = null)
		{
			var propertyValue = (T)context.GetType().GetProperty(propertyName).GetValue(context);
			return allowedValues.Contains(propertyValue);
		}

		public void Setup()
		{
			allowedValues = new List<T>();
		}

		private IEnumerable<T> GetPossibleValues()
		{
			return Enum.GetValues(typeof(T)).Cast<T>();
		}
	}
}