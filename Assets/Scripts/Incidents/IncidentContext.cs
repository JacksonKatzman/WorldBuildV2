using Game.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{
	public class IncidentContext
	{
		public IIncidentInstigator instigator;
		public List<IIncidentTag> tags;
		public IncidentContext(IIncidentInstigator instigator, List<IIncidentTag> tags)
		{
			this.instigator = instigator;
			this.tags = tags;
		}
	}
}