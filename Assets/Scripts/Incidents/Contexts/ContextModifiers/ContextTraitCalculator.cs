using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class ContextTraitCalculator<T> : IContextModifierCalculator where T : ContextTrait
	{
        public Type PrimitiveType => typeof(List<T>);

        public Type ContextType { get; set; }
        public int ID { get; set; }

        public string NameID => "{EX " + ID + "}";

        [HorizontalGroup("Group 1", 150), HideLabel, ReadOnly]
        public string propertyName;

        [LabelText("Add tag: True to add, false to remove.")]
        public bool addTrait = true;

        public ScriptableObjectRetriever<T> trait;
        public T Trait => (T)trait.RetrieveObject();

        public ContextTraitCalculator() { }

        public ContextTraitCalculator(string propertyName, Type contextType)
        {
            this.propertyName = propertyName;
            ContextType = contextType;
            
            var dataType = new Type[] { typeof(T) };
            var genericBase = typeof(ScriptableObjectRetriever<>);
            var combinedType = genericBase.MakeGenericType(dataType);
            trait = (ScriptableObjectRetriever<T>)Activator.CreateInstance(combinedType);
            
        }

        public void Calculate(IIncidentContext context)
        {
            var property = context.GetType().GetProperty(propertyName);
            var propertyValue = (List<T>)property.GetValue(context);
            if (addTrait && !propertyValue.Contains(Trait))
            {
                var p = new object[1] { Trait };
                propertyValue.GetType().GetMethod("Add").Invoke(property.GetValue(context), p);
            }
            else if (!addTrait && propertyValue.Contains(Trait))
            {
                var p = new object[1] { Trait };
                propertyValue.GetType().GetMethod("Remove").Invoke(property.GetValue(context), p);
            }
        }
    }
}