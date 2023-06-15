using Game.Incidents;
using UnityEngine;

namespace Game.Simulation
{
	public class IncidentContextDictionary : TypeListDictionary<IIncidentContext>
	{
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
	}
}
