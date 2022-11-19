using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class InterfacedIncidentActionFieldContainer<T> : IncidentActionFieldContainer
	{
		protected override IEnumerable<Type> GetFilteredTypeList()
		{
			return base.GetFilteredTypeList().Where(x => typeof(T).IsAssignableFrom(x));
		}
	}
}