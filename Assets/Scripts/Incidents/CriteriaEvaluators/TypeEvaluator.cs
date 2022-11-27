using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class TypeEvaluator : CriteriaEvaluator<Type>
	{
		protected override bool UseExpressions => false;

		[ValueDropdown("GetFilteredTypeList"), LabelText("Compare To Type")]
		public Type comparedType;
		public TypeEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }
		public override void Setup()
		{
			Comparators = ExpressionHelpers.TypeComparators;
		}

		protected override bool Evaluate(IIncidentContext context, string propertyName)
		{
			var propertyValue = (Type)context.GetType().GetProperty(propertyName).GetValue(context);
			return Comparators[Comparator].Invoke(propertyValue, comparedType);
		}

		private IEnumerable<Type> GetFilteredTypeList()
		{
			var q = ContextType.Assembly.GetTypes()
				.Where(x => !x.IsAbstract)                                          // Excludes BaseClass
				.Where(x => !x.IsGenericTypeDefinition)                             // Excludes Generics
				.Where(x => ContextType.IsAssignableFrom(x));           // Excludes classes not inheriting from IIncidentContext

			return q;
		}
	}
}