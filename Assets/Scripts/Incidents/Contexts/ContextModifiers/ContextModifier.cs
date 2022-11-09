using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
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
}