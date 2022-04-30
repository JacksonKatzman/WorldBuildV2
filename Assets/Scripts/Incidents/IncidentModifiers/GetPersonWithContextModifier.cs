using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public class GetPersonWithContextModifier : IncidentModifier
	{
		public List<SearchContext<Person>> criteria;

		public GetPersonWithContextModifier(List<IIncidentTag> tags, float probability) : base(tags, probability) { }

		public override void Setup()
		{
			base.Setup();

			var possiblePeople = SimulationManager.Instance.World.People;
			var matches = new HashSet<Person>();
			//var preAdd = criteria.ForEach(x => x.EvaluateSearch(possiblePeople));
			foreach(var c in criteria)
			{
				var eval = c.EvaluateSearch(possiblePeople);
				eval.ForEach(x => matches.Add(x));
			}
			var chosen = matches.ElementAt(SimRandom.RandomRange(0, matches.Count));

			ProvideModifierInfo(x => (x as ICreatureContainer)?.Creatures.Add(chosen));
		}
		public override void Modify(Action<IModifierInfoContainer> action)
		{
			base.Modify(action);
			criteria.ForEach(x => action.Invoke(x));
		}
	}
}