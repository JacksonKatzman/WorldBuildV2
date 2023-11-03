using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Generators.Names
{
	public abstract class AbstractNamingThemeObject : SerializedScriptableObject
	{
		protected Dictionary<string, List<string>> listCollection;
		protected Dictionary<string, List<string>> GetListCollection()
		{
			var allfields = GetType().GetFields().ToList();
			var lists = allfields.Where(x => x.FieldType == typeof(List<string>)).ToList();
			var stringLists = lists.Where(x => x.FieldType.GenericTypeArguments[0] == typeof(string));
			var dict = new Dictionary<string, List<string>>();
			foreach (var field in stringLists)
			{
				dict.Add(field.Name, (List<string>)field.GetValue(this));
			}
			return dict;
		}

		protected IEnumerable<string> GetListOptions()
		{
			if(listCollection == null)
			{
				listCollection = GetListCollection();
			}
			return listCollection.Keys;
		}

		[SerializeField, ValueDropdown("GetListOptions", IsUniqueList = true), PropertyOrder(Order = 1000)]
		private string listToAddTo;

		[TextArea, PropertyOrder(Order = 1001)]
		public string input;
		
		[Button("Add Items"), PropertyOrder(Order = 1002)]
		private void AddItems()
		{
			var split = input.Split('\n');
			if(listCollection.TryGetValue(listToAddTo, out var list))
			{
				foreach(var item in split)
				{
					if(!list.Contains(item))
					{
						list.Add(item);
					}
				}
			}
		}
	}
}
