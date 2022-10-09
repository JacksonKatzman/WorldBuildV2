using Game.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{
	public class OldIncidentContext
	{
		public IIncidentInstigator instigator;
		public List<IIncidentTag> tags;
		public OldIncidentContext(IIncidentInstigator instigator, List<IIncidentTag> tags)
		{
			this.instigator = instigator;
			this.tags = tags;
		}
	}
}