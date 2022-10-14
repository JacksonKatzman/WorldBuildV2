using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class GetCityWithContextModifier : IncidentModifier
	{
		public List<SearchContext<City>> criteria;

		public GetCityWithContextModifier(List<IIncidentTag> tags, float probability) : base(tags, probability) { }

		public override void Setup()
		{
			base.Setup();

			var possibleCities = OldSimulationManager.Instance.World.Cities;
			var matches = new HashSet<City>();
			//var preAdd = criteria.ForEach(x => x.EvaluateSearch(possiblePeople));
			foreach (var c in criteria)
			{
				var eval = c.EvaluateSearch(possibleCities);
				eval.ForEach(x => matches.Add(x));
			}
			var chosen = matches.ElementAt(SimRandom.RandomRange(0, matches.Count));

			ProvideModifierInfo(x => (x as ILandmarkContainer)?.Landmarks.Add(chosen));
		}
		public override void Modify(Action<IModifierInfoContainer> action)
		{
			base.Modify(action);
			criteria.ForEach(x => action.Invoke(x));
		}
	}
}