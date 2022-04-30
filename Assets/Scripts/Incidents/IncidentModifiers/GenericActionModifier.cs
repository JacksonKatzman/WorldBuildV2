using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class GenericActionModifier : IncidentModifier
	{
		public GenericActionModifier() : base(new List<IIncidentTag>(), 0)
		{
			//names = GetMethodNames();
		}
		public GenericActionModifier(List<IIncidentTag> tags, float probability) : base(tags, probability)
		{
			//methodNames = GetMethodNames();
		}
	}
}