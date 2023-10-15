using Game.Simulation;

namespace Game.Incidents
{
	public class ChangeAffiliatedFactionAction : GenericIncidentAction
	{
		public InterfacedIncidentActionFieldContainer<IFactionAffiliated> affiliate;
		public InterfacedIncidentActionFieldContainer<IFactionAffiliated> newFaction;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			affiliate.GetTypedFieldValue().AffiliatedFaction = newFaction.GetTypedFieldValue().AffiliatedFaction;
			EventManager.Instance.Dispatch(new AffiliatedFactionChangedEvent(affiliate.GetTypedFieldValue(), newFaction.GetTypedFieldValue()));
		}
	}
}