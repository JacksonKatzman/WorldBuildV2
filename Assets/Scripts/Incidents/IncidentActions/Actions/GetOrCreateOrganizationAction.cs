using Sirenix.OdinInspector;

namespace Game.Incidents
{
	public class GetOrCreateOrganizationAction : GetOrCreateAction<Organization>
	{
		[ShowIf("@this.allowCreate")]
		public InterfacedIncidentActionFieldContainer<IFactionAffiliated> faction;

		[ShowIf("@this.allowCreate")]
		public InterfacedIncidentActionFieldContainer<ISentient> creator;

		[ShowIf("@this.allowCreate")]
		public ContextualIncidentActionField<Landmark> baseOfOperations;

		[ShowIf("@this.allowCreate")]
		public ScriptableObjectRetriever<OrganizationTemplate> template = new ScriptableObjectRetriever<OrganizationTemplate>();

		protected override Organization MakeNew()
		{
			var fact = faction.GetTypedFieldValue().AffiliatedFaction;
			var temp = template.RetrieveObject();
			var org = new Organization(temp, fact, fact.AffiliatedRace, creator.GetTypedFieldValue());
			return org;
		}

		protected override void Complete()
		{
			if (madeNew)
			{
				baseOfOperations.GetTypedFieldValue().AffiliatedOrganization = actionField.GetTypedFieldValue();
				base.Complete();
			}
		}
		protected override bool VersionSpecificVerify(IIncidentContext context)
		{
			return faction.actionField.CalculateField(context) && creator.actionField.CalculateField(context) && baseOfOperations.CalculateField(context);
		}

		override protected void OnAllowCreateValueChanged()
		{
			faction.enabled = allowCreate;
			creator.enabled = allowCreate;
		}
	}
}