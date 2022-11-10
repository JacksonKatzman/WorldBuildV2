using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public abstract class ContextModifierCalculator<T> : IContextModifierCalculator
    {
        [HideInInspector]
        public Type Type => typeof(T);
        public Type ContextType { get; set; }

        protected Dictionary<string, Func<T, T, T>> Operators { get; set; }

        [HorizontalGroup("Group 1", 150), HideLabel, ReadOnly]
        public string propertyName;

        [ValueDropdown("GetOperatorNames"), OnValueChanged("SetOperatorType"), HorizontalGroup("Group 1", 50), HideLabel]
        public string Operation;

        [ListDrawerSettings(CustomAddFunction = "AddNewExpression"), HorizontalGroup("Group 1"), HideReferenceObjectPicker]
        public List<Expression<T>> expressions;

        virtual public bool AllowMultipleExpressions => true;

        public ContextModifierCalculator()
        {
            Setup();
        }

        public ContextModifierCalculator(string propertyName, Type contextType) : this()
        {
            this.propertyName = propertyName;
            ContextType = contextType;
            expressions = new List<Expression<T>>();
            expressions.Add(new Expression<T>(ContextType));
        }

        public void Calculate(IIncidentContext context)
        {
            var property = context.GetType().GetProperty(propertyName);
            var propertyValue = (T)property.GetValue(context);
            var calculatedValue = Operators[Operation].Invoke(propertyValue, CombineExpressions(context));
            property.SetValue(context, calculatedValue);
        }
        public T CombineExpressions(IIncidentContext context)
        {
            var currentValue = expressions[0].GetValue(context);
            for (int i = 0; i < expressions.Count - 1; i++)
            {
                currentValue = Operators[expressions[i].nextOperator].Invoke(currentValue, expressions[i + 1].GetValue(context));
            }
            return currentValue;
        }

        abstract public void Setup();

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

        private List<string> GetOperatorNames()
        {
            return Operators.Keys.ToList();
        }

        private void SetOperatorType()
        {

        }
    }
}