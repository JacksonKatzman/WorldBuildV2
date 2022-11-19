using Sirenix.OdinInspector;

namespace Game.Incidents
{
	abstract public class GetOrCreateAction<T> : GenericIncidentAction where T : IIncidentContext
	{
		public bool findFirst = true;

		[ShowIf("@this.findFirst")]
		public ContextualIncidentActionField<T> valueToFind;

		[PropertyOrder(10)]
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