using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class ContextTagCalculator<T> : IContextModifierCalculator where T : IContextTag
	{
        public Type PrimitiveType => typeof(List<T>);

        public Type ContextType { get; set; }
        public int ID { get; set; }

        public string NameID => "{EX " + ID + "}";

        [HorizontalGroup("Group 1", 150), HideLabel, ReadOnly]
        public string propertyName;

        [LabelText("Add tag: True to add, false to remove.")]
        public bool addTag = true;

        public T tag;

        public ContextTagCalculator() { }

        public ContextTagCalculator(string propertyName, Type contextType)
        {
            this.propertyName = propertyName;
            ContextType = contextType;
            tag = (T)Activator.CreateInstance(typeof(T));
        }

        public void Calculate(IIncidentContext context)
        {
            var property = context.GetType().GetProperty(propertyName);
            var propertyValue = (List<T>)property.GetValue(context);
            if (addTag && !propertyValue.Contains(tag))
            {
                //propertyValue.Add(tag);
                var p = new object[1] { tag };
                propertyValue.GetType().GetMethod("Add").Invoke(property.GetValue(context), p);
            }
            else if (!addTag && propertyValue.Contains(tag))
            {
                var p = new object[1] { tag };
                propertyValue.GetType().GetMethod("Remove").Invoke(property.GetValue(context), p);
            }
        }
    }
}