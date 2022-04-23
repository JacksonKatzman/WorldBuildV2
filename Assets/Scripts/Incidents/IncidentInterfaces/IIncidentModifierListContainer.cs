using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public interface IIncidentModifierListContainer
	{
		public List<IncidentModifier> Incidents
		{
			get;
		}
		//public List<IncidentModifier> Required
		public void Modify(Action<IncidentModifier> action);
		public void ReplaceModifier(IncidentModifier replaceWith, int replaceID);
	}
}