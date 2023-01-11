using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Game.Data
{
	[Serializable]
	public class WeightedListSerializableDictionary<T> : UnitySerializedDictionary<int, List<T>>
	{
		public WeightedListSerializableDictionary() : base() { }
		public WeightedListSerializableDictionary(WeightedListSerializableDictionary<T> a) : this()
		{
			foreach(var pair in a)
			{
				this.Add(pair.Key, pair.Value);
			}
		}

		public static WeightedListSerializableDictionary<T> Merge(WeightedListSerializableDictionary<T> a, WeightedListSerializableDictionary<T> b)
		{
			var result = new WeightedListSerializableDictionary<T>(a);

			foreach (var pair in b)
			{
				if (a.ContainsKey(pair.Key))
				{
					result.Add(pair.Key, a[pair.Key].Union(pair.Value).ToList());
				}
				else
				{
					a.Add(pair.Key, pair.Value);
				}
			}

			return result;
		}
		public void Merge(WeightedListSerializableDictionary<T> other)
		{
			foreach(var pair in other)
			{
				if(this.ContainsKey(pair.Key))
				{
					this[pair.Key] = this[pair.Key].Union(pair.Value).ToList();
				}
				else
				{
					this.Add(pair.Key, pair.Value);
				}
			}
		}
	}

	public abstract class UnitySerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		[SerializeField, HideInInspector]
		private List<TKey> keyData = new List<TKey>();

		[SerializeField, HideInInspector]
		private List<TValue> valueData = new List<TValue>();

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.Clear();
			for (int i = 0; i < this.keyData.Count && i < this.valueData.Count; i++)
			{
				this[this.keyData[i]] = this.valueData[i];
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			this.keyData?.Clear();
			this.valueData?.Clear();

			foreach (var item in this)
			{
				this.keyData.Add(item.Key);
				this.valueData.Add(item.Value);
			}
		}
	}
}