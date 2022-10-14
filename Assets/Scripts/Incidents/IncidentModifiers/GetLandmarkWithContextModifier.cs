using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class GetLandmarkWithContextModifier : IncidentModifier
	{
		public List<SearchContext<ILandmark>> criteria;

		public GetLandmarkWithContextModifier(List<IIncidentTag> tags, float probability) : base(tags, probability) { }

		public override void Setup()
		{
			base.Setup();

			var possibleLandmarks = OldSimulationManager.Instance.World.landmarks;
			var matches = new HashSet<ILandmark>();
			//var preAdd = criteria.ForEach(x => x.EvaluateSearch(possiblePeople));
			foreach (var c in criteria)
			{
				var eval = c.EvaluateSearch(possibleLandmarks);
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