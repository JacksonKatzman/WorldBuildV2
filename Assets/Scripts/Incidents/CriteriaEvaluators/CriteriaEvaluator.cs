using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game.Incidents
{
	abstract public class CriteriaEvaluator<T> : ICriteriaEvaluator
	{
        [HideInInspector]
        public Type Type => typeof(T);
        public Type ContextType { get; set; }

        private string comparator;

        protected Dictionary<string, Func<T, T, bool>> Comparators { get; set; }
        protected Dictionary<string, Func<T, T, T>> Operators { get; set; }

        [HorizontalGroup("Group 1", 150), HideLabel, ReadOnly]
        public string propertyName;

        [ValueDropdown("GetComparatorNames"), OnValueChanged("SetComparatorType"), HorizontalGroup("Group 1", 50), HideLabel]
        public string Comparator;

        [ListDrawerSettings(CustomAddFunction = "AddNewExpression"), HorizontalGroup("Group 1"), HideReferenceObjectPicker]
        public List<Expression<T>> expressions;
        virtual public bool AllowMultipleExpressions => true;

        virtual public bool Evaluate(IIncidentContext context, string propertyName)
        {
            var propertyValue = (T)context.GetType().GetProperty(propertyName).GetValue(context);
            return Comparators[Comparator].Invoke(propertyValue, CombineExpressions(context));
        }

        public CriteriaEvaluator()
        {
            Setup();
        }

        public CriteriaEvaluator(string propertyName) : this()
		{
            this.propertyName = propertyName;
        }

        public CriteriaEvaluator(string propertyName, T value) : this(propertyName)
		{
            expressions[0].constValue = value;
        }

        public CriteriaEvaluator(string propertyName, Type contextType) : this(propertyName)
		{
            ContextType = contextType;
            expressions = new List<Expression<T>>();
            expressions.Add(new Expression<T>(ContextType));
        }

        abstract protected void Setup();

        public T CombineExpressions(IIncidentContext context)
        {
            var currentValue = expressions[0].GetValue(context);
            for (int i = 0; i < expressions.Count - 1; i++)
            {
                currentValue = Operators[expressions[i].nextOperator].Invoke(currentValue, expressions[i + 1].GetValue(context));
            }
            return currentValue;
        }

        private void AddNewExpression()
        {
            if (AllowMultipleExpressions)
            {
                expressions.Add(new Expression<T>(ContextType));
                for (int i = 0; i < expressions.Count - 1; i++)
                {
                    expressions[i].hasNextOperator = true;
                }
                expressions[expressions.Count - 1].hasNextOperator = false;
            }
        }

        private List<string> GetComparatorNames()
        {
            return Comparators.Keys.ToList();
        }

        private void SetComparatorType()
        {
            comparator = Comparator;
        }
    }
}