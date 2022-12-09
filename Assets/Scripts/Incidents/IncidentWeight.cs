using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public class IncidentWeight<T> : IIncidentWeight where T : IIncidentContext
	{
		[Range(0, 20)]
		public int baseWeight;

		[ValueDropdown("GetOperatorNames"), HorizontalGroup("Group 1", 50), HideLabel]
		public string Operation;

		[ListDrawerSettings(CustomAddFunction = "AddNewExpression"), HideReferenceObjectPicker]
		public List<Expression<int>> expressions;

		protected Dictionary<string, Func<int, int, int>> Operators { get; set; }

		public IncidentWeight()
		{
			Operators = ExpressionHelpers.IntegerOperators;
			expressions = new List<Expression<int>>();
		}

		public IncidentWeight(int baseWeight) : this()
		{
			this.baseWeight = baseWeight;
		}

		public int CalculateWeight(IIncidentContext context)
		{
			if (expressions.Count > 0 && context != null)
			{
				return Operators[Operation].Invoke(baseWeight, Expression<int>.CombineExpressions(context, expressions, Operators));
			}
			else
			{
				return baseWeight;
			}
		}

		private void AddNewExpression()
		{
			expressions.Add(new Expression<int>(typeof(T)));
		}

		private List<string> GetOperatorNames()
		{
			return Operators.Keys.ToList();
		}
	}
}