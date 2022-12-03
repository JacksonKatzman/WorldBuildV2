﻿using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
    public interface IContextModifier
    {
        [ShowInInspector]
        IContextModifierCalculator Calculator { get; set; }
        void Modify(IIncidentContext context);
    }
	public class ContextModifier<T> : IContextModifier
	{
		public Type ContextType => typeof(T);
		public Type PrimitiveType { get; set; }
		private Dictionary<string, Type> properties;

		[ValueDropdown("GetPropertyNames"), OnValueChanged("SetPrimitiveType"), HorizontalGroup("Group 1")]
		public string propertyName;

        [ShowInInspector, ShowIfGroup("PropertyChosen"), HideReferenceObjectPicker]
        public IContextModifierCalculator Calculator { get; set; }

        private bool PropertyChosen => propertyName != null;

        public ContextModifier()
		{
            GetPropertyList();
		}

        public void Modify(IIncidentContext context)
		{
            Calculator.Calculate(context);
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
                Calculator = new IntegerContextModifierCalculator(propertyName, ContextType);
            }
            else if (PrimitiveType == typeof(float))
            {
                Calculator = new FloatContextModifierCalculator(propertyName, ContextType);
            }
            else if (PrimitiveType == typeof(bool))
            {
                Calculator = new BooleanContextModifierCalculator(propertyName, ContextType);
            }
            IncidentEditorWindow.UpdateActionFieldIDs();
        }
    }
}