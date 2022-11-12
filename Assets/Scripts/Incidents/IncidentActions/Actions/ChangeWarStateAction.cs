namespace Game.Incidents
{
	public class ChangeWarStateAction : GenericIncidentAction
	{
		public ContextualIncidentActionField<Faction> factionOne;
		public ContextualIncidentActionField<Faction> factionTwo;
		public bool atWar;

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			var f1 = factionOne.GetTypedFieldValue();
			var f2 = factionTwo.GetTypedFieldValue();
			if(atWar)
			{
				f1.FactionsAtWarWith.Add(f2);
				f2.FactionsAtWarWith.Add(f1);
			}
			else
			{
				if(f1.FactionsAtWarWith.Contains(f2))
				{
					f1.FactionsAtWarWith.Remove(f2);
				}
				if (f2.FactionsAtWarWith.Contains(f1))
				{
					f2.FactionsAtWarWith.Remove(f1);
				}
			}
		}
	}
}