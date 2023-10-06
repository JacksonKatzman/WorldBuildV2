

using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Incidents
{
	[HideReferenceObjectPicker]
	public class IndexedObject<T>
	{
		[HideInInspector]
		public T obj;
		[ReadOnly]
		readonly public int index;

		public IndexedObject() { }

		public IndexedObject(int index)
		{
			this.index = index;
		}
	}
}