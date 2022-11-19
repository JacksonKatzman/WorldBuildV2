namespace Game.Incidents
{
	abstract public class GetOrCreateAction<T> : GenericIncidentAction where T : IIncidentContext
	{
		public bool findFirst = true;
		public ContextualIncidentActionField<T> valueToFind;
		public ActionResultField<T> result;

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			if(!findFirst || valueToFind.GetTypedFieldValue() == null)
			{
				MakeNew();
			}
			else
			{
				result.SetValue(valueToFind.GetTypedFieldValue());
			}
		}

		abstract protected void MakeNew();
	}
}