using Game.Incidents;

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
	}
}
