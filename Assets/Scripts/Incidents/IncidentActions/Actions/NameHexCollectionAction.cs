using Sirenix.OdinInspector;

namespace Game.Incidents
{
	public class NameHexCollectionAction : GenericIncidentAction
	{
		public InterfacedIncidentActionFieldContainer<ILocationAffiliated> location;
		public InterfacedIncidentActionFieldContainer<ISentient> namedAfter;

		public bool useCustomFormat;

		[ShowIf("@this.useCustomFormat")]
		public string customFormat;

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			
		}
	}
}