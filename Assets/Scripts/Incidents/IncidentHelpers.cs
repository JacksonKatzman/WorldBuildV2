using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public static class IncidentHelpers
	{
		public static IEnumerable<string> GetMethodNames(Type type)
		{
			var methods = (type).GetMethods().Where(m => !typeof(object)
									 .GetMethods()
									 .Select(me => me.Name)
									 .Contains(m.Name)).ToList();
			var names = new List<string>();
			methods.ForEach(x => names.Add(x.Name));
			return names;
		}

		public static IEnumerable<Type> GetInterfaceTypes(Type type)
		{
			var types = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(s => s.GetTypes())
			.Where(p => type.IsAssignableFrom(p));
			return types.ToList();
		}
	}
}