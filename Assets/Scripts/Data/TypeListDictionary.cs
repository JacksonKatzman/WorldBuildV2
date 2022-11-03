using System;
using System.Collections.Generic;

namespace Game.Simulation
{
	public class TypeListDictionary<T> : Dictionary<Type, List<T>>
	{
		public new List<T> this[Type key]
		{
			get
			{
				if(!this.ContainsKey(key))
				{
					this.Add(key, new List<T>());
				}

				return base[key];
			}
			set
			{
				if (!this.ContainsKey(key))
				{
					this.Add(key, new List<T>());
				}

				base[key] = value;
			}
		}
	}
}
