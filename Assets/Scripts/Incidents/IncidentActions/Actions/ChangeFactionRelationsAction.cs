using Game.Data;

namespace Game.Incidents
{
	public class ChangeFactionRelationsAction : GenericIncidentAction
	{
		public InterfacedIncidentActionFieldContainer<IFactionAffiliated> affectedFaction;
		public InterfacedIncidentActionFieldContainer<IFactionAffiliated> otherFaction;
		public IntegerRange amount;
		public bool set;
		public bool mirrored;

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			var factionA = affectedFaction.GetTypedFieldValue().AffiliatedFaction;
			var factionB = otherFaction.GetTypedFieldValue().AffiliatedFaction;
			if(factionA != factionB)
			{
				if(!factionA.FactionRelations.ContainsKey(factionB))
				{
					factionA.FactionRelations.Add(factionB, 0);
				}

				factionA.FactionRelations[factionB] = set ? amount : factionA.FactionRelations[factionB] + amount;

				if (mirrored)
				{
					if (!factionB.FactionRelations.ContainsKey(factionA))
					{
						factionB.FactionRelations.Add(factionA, 0);
					}

					factionB.FactionRelations[factionA] = set ? amount : factionB.FactionRelations[factionA] + amount;
				}
			}
		}
	}
}