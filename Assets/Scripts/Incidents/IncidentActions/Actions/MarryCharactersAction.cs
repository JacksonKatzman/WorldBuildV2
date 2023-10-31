namespace Game.Incidents
{
	public class MarryCharactersAction : GenericIncidentAction
	{
		public InterfacedIncidentActionFieldContainer<Character> initiator;
		public InterfacedIncidentActionFieldContainer<Character> recipient;

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			Character.HandleMarriage(initiator.GetTypedFieldValue(), recipient.GetTypedFieldValue());
		}
	}
}