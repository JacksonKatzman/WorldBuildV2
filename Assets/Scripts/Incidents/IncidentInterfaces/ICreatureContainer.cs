using Game.Creatures;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{
	public interface ICreatureContainer
	{
		public List<ICreature> Creatures
		{
			get;
		}
	}
}