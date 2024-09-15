using Game.Incidents;
using System;

namespace Game.Generators.Items
{
    public class Disease : InertIncidentContext
	{
		public override Type ContextType => typeof(Disease);
		public override string Description => $"DISEASE DESCRIPTION";
	}
}