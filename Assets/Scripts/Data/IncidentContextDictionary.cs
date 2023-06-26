using Game.Generators.Items;
using Game.Incidents;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Simulation
{
	public class IncidentContextDictionary : TypeListDictionary<IIncidentContext>
	{
		public IncidentContextDictionary()
		{
			var types = GetRequiredTypes();
			foreach(var type in types)
			{
				this.Add(type, new List<IIncidentContext>());
			}
		}
		public IIncidentContext GetContextByID(int id)
		{
			foreach(var list in this.Values)
			{
				foreach(var item in list)
				{
					if(item.ID == id)
					{
						return item;
					}
				}
			}

			return null;
		}

		public T GetContextByID<T>(int id)
		{
			if(this.ContainsKey(typeof(T)))
			{
				var list = this[typeof(T)];
				foreach (var item in list)
				{
					if (item.ID == id)
					{
						return (T)item;
					}
				}
			}

			return default(T);
		}

		public void LoadContextProperties()
		{
			foreach (var list in this.Values)
			{
				foreach (var item in list)
				{
					item.LoadContextProperties();
				}
			}
		}

		private List<Type> GetRequiredTypes()
		{
			return new List<Type>()
			{
				typeof(Character), typeof(Faction),
				typeof(Item), typeof(City),
				typeof(Location), typeof(Landmark),
				typeof(Race), typeof(Monster),
				typeof(Resource), typeof(GreatMonster),
				typeof(Organization)
			};
		}

		/*
		private IEnumerable<Type> GetAllMatchingContextTypes(Type type)
		{
			var q = type.Assembly.GetTypes()
				.Where(x => !x.IsAbstract)                                          // Excludes BaseClass
				.Where(x => !x.IsGenericTypeDefinition)                             // Excludes Generics
				.Where(x => type.IsAssignableFrom(x));

			return q;// Excludes classes not inheriting from IIncidentContext
		}
		*/
	}
}
