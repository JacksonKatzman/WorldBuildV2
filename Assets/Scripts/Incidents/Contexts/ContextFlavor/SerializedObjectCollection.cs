using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	[CreateAssetMenu(fileName = nameof(SerializedObjectCollection), menuName = "ScriptableObjects/Data/" + nameof(SerializedObjectCollection), order = 1)]
	public class SerializedObjectCollection : SerializedScriptableObject
	{
		[ValueDropdown("GetFilteredTypeList")]
		public Type objectType;

		[ReadOnly]
		public Dictionary<string, SerializedScriptableObject> objects;

		public string resourcePath;

		[Button("Compile Object Dictionary")]
		private void CompileObjectDictionary()
		{
			var resources = Resources.LoadAll("ScriptableObjects/" + resourcePath, objectType);
			foreach(var resource in resources)
			{
				var typedResource = resource as SerializedScriptableObject;
				if(objects == null)
				{
					objects = new Dictionary<string, SerializedScriptableObject>();
				}

				if(!objects.ContainsKey(typedResource.name))
				{
					objects.Add(typedResource.name, typedResource);
				}
			}
		}

		[Button("Clear Dictionary")]
		private void Clear()
		{
			objects.Clear();
		}

		private IEnumerable<Type> GetFilteredTypeList()
		{
			var q = typeof(IIncidentContext).Assembly.GetTypes()
				.Where(x => !x.IsAbstract)                                          // Excludes BaseClass
				.Where(x => !x.IsGenericTypeDefinition)                             // Excludes Generics
				.Where(x => typeof(SerializedScriptableObject).IsAssignableFrom(x));

			return q;
		}
	}
}