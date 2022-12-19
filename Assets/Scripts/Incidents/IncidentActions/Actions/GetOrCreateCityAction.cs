using Game.Simulation;
using Sirenix.OdinInspector;

namespace Game.Incidents
{
	public class GetOrCreateCityAction : GetOrCreateAction<City>
	{
		[ShowIf("@this.allowCreate")]
		public ContextualIncidentActionField<Faction> faction;
		[ShowIf("@this.allowCreate")]
		public LocationActionField location;
		[ShowIf("@this.allowCreate")]
		public IntegerRange population;
		[ShowIf("@this.allowCreate")]
		public IntegerRange wealth;
		protected override City MakeNew()
		{
			var newCity = new City(faction.GetTypedFieldValue(), location.GetTypedFieldValue(), population, wealth);
			faction.GetTypedFieldValue().Cities.Add(newCity);

			return newCity;
		}

		protected override bool VersionSpecificVerify(IIncidentContext context)
		{
			return faction.CalculateField(context) && location.CalculateField(context);
		}
	}
}