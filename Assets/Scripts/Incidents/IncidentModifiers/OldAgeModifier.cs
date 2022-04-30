using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public class OldAgeModifier : IncidentModifier
	{
		public OldAgeModifier() : base(new List<IIncidentTag>(), 0)
		{

		}
		public OldAgeModifier(List<IIncidentTag> tags, float probability) : base(tags, probability)
		{
		}

		public override void Setup()
		{
			//var people = SimulationManager.Instance.World.People.Where(x => x.age >= x.naturalDeathAge).ToList();
			//(coreIncident.core as ICreatureContainer)?.Creatures.AddRange(people);
			//ProvideModifierInfo((x) => (x as ICreatureContainer)?.Creatures.AddRange(people));
			ProvideModifierInfo(CreateModifierInfo);
		}

		public void CreateModifierInfo(IModifierInfoContainer mod)
		{
			var people = SimulationManager.Instance.World.People.Where(x => x.age >= x.naturalDeathAge).ToList();
			var container = (mod as ICreatureContainer);
			var creatures = container?.Creatures;
			creatures?.AddRange(people);
		}
	}
}