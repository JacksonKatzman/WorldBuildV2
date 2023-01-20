namespace Game.Incidents
{
	public class ModifyFactionOrganizationAction : GenericIncidentAction
	{
		public ContextualIncidentActionField<Faction> faction;
		public Enums.OrganizationType organizationType;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			faction.GetTypedFieldValue().Government.UpdateHierarchy();
		}
	}
}