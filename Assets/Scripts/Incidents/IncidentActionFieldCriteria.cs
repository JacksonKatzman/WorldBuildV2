using Game.Enums;
using Game.Generators.Items;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class IncidentActionFieldCriteria : IncidentCriteria
	{
        public IncidentActionFieldCriteria() { }
        public IncidentActionFieldCriteria(Type contextType) : base(contextType) { }

		protected override bool IsValidPropertyType(Type type)
		{
            return base.IsValidPropertyType(type) || type == typeof(Faction) || type == typeof(Location) || type == typeof(Inventory) || type == typeof(Type) || type.IsEnum
			|| ((type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) && typeof(IIncidentContext).IsAssignableFrom(type.GetGenericArguments()[0])));
		}

		protected override void SetPrimitiveType()
		{
			base.SetPrimitiveType();

			if (PrimitiveType == typeof(Faction))
			{
				evaluator = new FactionEvaluator(propertyName, ContextType);
			}
			else if(PrimitiveType == typeof(Location))
			{
				evaluator = new LocationEvaluator(propertyName, ContextType);
			}
			else if(PrimitiveType == typeof(Inventory))
			{
				evaluator = new InventoryEvaluator(propertyName, ContextType);
			}
			else if (PrimitiveType == typeof(Gender))
			{
				evaluator = new GenderEvaluator(propertyName, ContextType);
			}
			else if(PrimitiveType == typeof(Type))
			{
				evaluator = new TypeEvaluator(propertyName, ContextType);
			}
			else if(PrimitiveType == typeof(Dictionary<IIncidentContext, int>))
			{
				evaluator = new ActionFieldIntDictionaryEvaluator(propertyName, ContextType);
			}
			else if(/*ContextType == IncidentEditorWindow.ContextType && */PrimitiveType == typeof(List<IIncidentContext>))
			{
				evaluator = new ActionFieldListContainsEvaluator(propertyName, ContextType);
			}
			else if(PrimitiveType.IsGenericType && PrimitiveType.GetGenericTypeDefinition() == typeof(List<>) && typeof(IIncidentContext).IsAssignableFrom(PrimitiveType.GetGenericArguments()[0]))
			{
				evaluator = new ActionFieldListContainsEvaluator(propertyName, ContextType);
			}
			else if(PrimitiveType.IsEnum)
			{
				var dataType = new Type[] { PrimitiveType };
				var genericBase = typeof(EnumEvaluator<>);
				var combinedType = genericBase.MakeGenericType(dataType);
				evaluator = (ICriteriaEvaluator)Activator.CreateInstance(combinedType, propertyName, ContextType);
			}
		}
	}
}