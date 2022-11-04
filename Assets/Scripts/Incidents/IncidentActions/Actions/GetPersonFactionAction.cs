namespace Game.Incidents
{
	public class GetPersonFactionAction : GenericIncidentAction
	{
		public ContextualIncidentActionField<Person> person;
		public ActionResultField<Faction> faction;

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			faction.SetValue(person.GetTypedFieldValue().Faction);
		}
	}
}