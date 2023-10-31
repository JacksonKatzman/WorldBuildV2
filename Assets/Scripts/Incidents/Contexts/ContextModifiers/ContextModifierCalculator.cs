﻿using Game.Enums;
using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public class CharacterTagCalculator : IContextModifierCalculator
	{
        public Type PrimitiveType => typeof(List<CharacterTag>);

        public int ID { get; set; }

        public string NameID => "{EX " + ID + "}";

        [HorizontalGroup("Group 1", 150), HideLabel, ReadOnly]
        public string propertyName;

        [LabelText("Add tag: True to add, false to remove.")]
        public bool addTag;

        public CharacterTag tag;

		public CharacterTagCalculator(string propertyName)
		{
			this.propertyName = propertyName;
		}

		public void Calculate(IIncidentContext context)
		{
            var property = context.GetType().GetProperty(propertyName);
            var propertyValue = (List<CharacterTag>)property.GetValue(context);
            if(addTag && !propertyValue.Contains(tag))
			{
                propertyValue.Add(tag);
			}
            else if(!addTag && propertyValue.Contains(tag))
			{
                propertyValue.Remove(tag);
			}
        }
	}
	public abstract class ContextModifierCalculator<T> : IContextModifierCalculator
    {
        [HideInInspector]
        public Type PrimitiveType => typeof(T);
        public Type ContextType { get; set; }
        public int ID { get; set; }
        [ReadOnly, ShowInInspector]
        public string NameID => "{EX " + ID + "}";

        protected Dictionary<string, Func<T, T, T>> Operators { get; set; }

        [HorizontalGroup("Group 1", 150), HideLabel, ReadOnly]
        public string propertyName;

        [ValueDropdown("GetOperatorNames"), OnValueChanged("SetOperatorType"), HorizontalGroup("Group 1", 50), HideLabel]
        public string Operation;

        [ListDrawerSettings(CustomAddFunction = "AddNewExpression"), HorizontalGroup("Group 1"), HideReferenceObjectPicker]
        public List<Expression<T>> expressions;

        public bool clamped = true;

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
            expressions.Add(new Expression<T>(ContextType, propertyName));
        }

        public void Calculate(IIncidentContext context)
        {
            var property = context.GetType().GetProperty(propertyName);
            var propertyValue = (T)property.GetValue(context);
            var combinedExpressions = CombineExpressions(context);
            ContextDictionaryProvider.CurrentExpressionValues.Add(NameID, new ExpressionValue(combinedExpressions));
            var calculatedValue = Operators[Operation].Invoke(propertyValue, combinedExpressions);
            if(clamped)
			{
                calculatedValue = Clamp(calculatedValue);
			}
            property.SetValue(context, calculatedValue);
        }
        public T CombineExpressions(IIncidentContext context)
        {
            return Expression<T>.CombineExpressions(context, expressions, Operators);
        }

        abstract public void Setup();

        abstract protected T Clamp(T value);

        private void AddNewExpression()
        {
            if (AllowMultipleExpressions)
            {
                expressions.Add(new Expression<T>(ContextType, propertyName));
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