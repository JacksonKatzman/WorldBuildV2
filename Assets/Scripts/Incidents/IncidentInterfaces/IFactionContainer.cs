using Game.Factions;
using System.Collections.Generic;

namespace Game.Incidents
{
	public interface IFactionContainer
	{
		public List<Faction> Factions
		{
			get;
		}
	}
}