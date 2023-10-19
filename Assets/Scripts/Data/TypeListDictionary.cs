using System;
using System.Collections.Generic;

namespace Game.Data
{
	public class TypeListDictionary<T> : Dictionary<Type, List<T>>
	{
		public new List<T> this[Type key]
		{
			get
			{
				if (!ContainsKey(key))
				{
					Add(key, new List<T>());
				}

				return base[key];
			}
			set
			{
				if (!ContainsKey(key))
				{
					Add(key, new List<T>());
				}

				base[key] = value;
			}
		}
	}
}
