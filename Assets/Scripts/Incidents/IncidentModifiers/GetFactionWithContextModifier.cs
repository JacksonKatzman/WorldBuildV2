using Game.Factions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class GetFactionWithContextModifier : IncidentModifier
	{
		public List<SearchContext<OldFaction>> criteria;

		public GetFactionWithContextModifier(List<IIncidentTag> tags, float probability) : base(tags, probability) { }

		public override void Setup()
		{
			base.Setup();

			var possibleFactions = OldSimulationManager.Instance.World.factions;
			var matches = new HashSet<OldFaction>();
			//var preAdd = criteria.ForEach(x => x.EvaluateSearch(possiblePeople));
			foreach (var c in criteria)
			{
				var eval = c.EvaluateSearch(possibleFactions);
				eval.ForEach(x => matches.Add(x));
			}
			var chosen = matches.ElementAt(SimRandom.RandomRange(0, matches.Count));

			ProvideModifierInfo(x => (x as IFactionContainer)?.Factions.Add(chosen));
		}
		public override void Modify(Action<IModifierInfoContainer> action)
		{
			base.Modify(action);
			criteria.ForEach(x => action.Invoke(x));
		}
	}
}