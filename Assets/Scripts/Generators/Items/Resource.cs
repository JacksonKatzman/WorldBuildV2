using Game.Incidents;
using System;

namespace Game.Generators.Items
{
	public class Resource : InertIncidentContext
	{
		public override Type ContextType => typeof(Resource);
	}

	public class Disease : InertIncidentContext
	{
		public override Type ContextType => typeof(Disease);
	}
}