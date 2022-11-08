using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	abstract public class ModifyContextAction<T> : GenericIncidentAction where T : IIncidentContext
	{
		public ContextualIncidentActionField<T> contextToModify;

        [ListDrawerSettings(CustomAddFunction = "AddNewModifier"), HorizontalGroup("Group 1"), HideReferenceObjectPicker]
        public List<ContextModifier<T>> modifiers;

        public ModifyContextAction() { }

        override public void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
            foreach(var modifier in modifiers)
			{
                modifier.Modify(contextToModify.GetTypedFieldValue());
			}
		}

		public override void UpdateEditor()
		{
			base.UpdateEditor();
            modifiers = new List<ContextModifier<T>>();
		}

		private void AddNewModifier()
		{
            modifiers.Add(new ContextModifier<T>());
		}
	}

    public class ModifyPersonAction : ModifyContextAction<Person>
	{
        public ModifyPersonAction() : base() { }
	}

	public class ContextModifier<T>
	{
		public Type ContextType => typeof(T);
		public Type PrimitiveType { get; set; }
		private Dictionary<string, Type> properties;

		[ValueDropdown("GetPropertyNames"), OnValueChanged("SetPrimitiveType"), HorizontalGroup("Group 1")]
		public string propertyName;

        [ShowIfGroup("PropertyChosen"), HideReferenceObjectPicker]
        public IContextModifierCalculator calculator;

        private bool PropertyChosen => propertyName != null;

        public ContextModifier()
		{
            GetPropertyList();
		}

        public void Modify(IIncidentContext context)
		{
            calculator.Calculate(context);
		}

        private void GetPropertyList()
        {
            if (properties == null)
            {
                properties = new Dictionary<string, Type>();
            }
            if (ContextType != null)
            {
                var propertyInfo = ContextType.GetProperties();
                var interfacePropertyInfo = typeof(IIncidentContext).GetProperties();

                var validProperties = propertyInfo.Where(x => !interfacePropertyInfo.Any(y => x.Name == y.Name) && IsValidPropertyType(x.PropertyType)).ToList();

                properties.Clear();

                validProperties.ForEach(x => properties.Add(x.Name, x.PropertyType));
            }
        }

        virtual protected bool IsValidPropertyType(Type type)
        {
            return type == typeof(int) || type == typeof(float) || type == typeof(bool);
        }

        private IEnumerable<string> GetPropertyNames()
        {
            if (properties == null || properties.Count == 0)
            {
                GetPropertyList();
            }
            return properties.Keys.ToList();
        }

        virtual protected void SetPrimitiveType()
        {
            PrimitiveType = properties[propertyName];

            if (PrimitiveType == typeof(int))
            {
                calculator = new IntegerContextModifierCalculator(propertyName, ContextType);
            }
            else if (PrimitiveType == typeof(float))
            {
                calculator = new FloatContextModifierCalculator(propertyName, ContextType);
            }
            else if (PrimitiveType == typeof(bool))
            {
                calculator = new BooleanContextModifierCalculator(propertyName, ContextType);
            }
        }
    }

    public interface IContextModifierCalculator 
    {
        void Calculate(IIncidentContext context);
    }

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

	public class IntegerContextModifierCalculator : ContextModifierCalculator<int>
	{
        public IntegerContextModifierCalculator(string propertyName, Type contextType) : base(propertyName, contextType) { }
		public override void Setup()
		{
            Operators = ExpressionHelpers.IntegerOperators;
		}
	}

	public class FloatContextModifierCalculator : ContextModifierCalculator<float>
	{
        public FloatContextModifierCalculator(string propertyName, Type contextType) : base(propertyName, contextType) { }
        public override void Setup()
        {
            Operators = ExpressionHelpers.FloatOperators;
        }
    }

    public class BooleanContextModifierCalculator : ContextModifierCalculator<bool>
	{
        public BooleanContextModifierCalculator(string propertyName, Type contextType) : base(propertyName, contextType) { }
        public override void Setup()
        {
            Operators = ExpressionHelpers.BoolOperators;
        }
    }
}