using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Game.Incidents
{
	static public class IncidentActionHelpers
	{
		static public IEnumerable<Type> GetAllTypesImplementingOpenGenericType(Type openGenericType, Assembly assembly)
		{
			return from x in assembly.GetTypes()
				   from z in x.GetInterfaces()
				   let y = x.BaseType
				   where
				   (y != null && y.IsGenericType &&
				   openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition())) ||
				   (z.IsGenericType &&
				   openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
				   select x;
		}

		static public IEnumerable<Type> GetAllTypesImplementingType(Type type, Assembly assembly)
		{
			return assembly
						.GetTypes()
						.Where(t => type.IsAssignableFrom(t) &&
									t != type);
		}

		static public IEnumerable<Type> GetFilteredTypeList(Type contextType)
		{
			var allActionsWithGenericParents = GetAllTypesImplementingOpenGenericType(typeof(IIncidentAction), Assembly.GetExecutingAssembly()).ToList();
			var contextualActions = allActionsWithGenericParents.Where(x => (x.BaseType.IsGenericType == true && x.BaseType.GetGenericArguments()[0] == contextType));
			var genericActions = GetAllTypesImplementingType(typeof(GenericIncidentAction), Assembly.GetExecutingAssembly());
			var matches = contextualActions.Concat(genericActions).ToList();
			return matches;
		}
	}
}