using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class IncidentActionBranchWeightModifier
	{
		public Type ContextType { get; set; }
		[ValueDropdown("GetPropertyList")]
		public string propertyName;

		public IncidentActionBranchWeightModifier() { }
		public IncidentActionBranchWeightModifier(Type type)
		{
			ContextType = type;
		}

		public int Evaluate(IIncidentContext context)
		{
			return (int)ContextType.GetProperty(propertyName).GetValue(context);
		}

		private IEnumerable<string> GetPropertyList()
		{
			var propertyInfo = ContextType.GetProperties();
			var interfacePropertyInfo = typeof(IIncidentContext).GetProperties();

			var validProperties = propertyInfo.Where(x => !interfacePropertyInfo.Any(y => x.Name == y.Name) && x.PropertyType == typeof(int));
			return validProperties.Select(x => x.Name);
		}
	}
}