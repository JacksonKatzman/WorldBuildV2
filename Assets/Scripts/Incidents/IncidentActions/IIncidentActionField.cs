using System;

namespace Game.Incidents
{
	public interface IIncidentActionField 
	{
		int ActionFieldID { get; set; }
		string ActionFieldIDString { get; }
		string NameID { get; set; }

		Type ContextType { get; }

		bool CalculateField(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction);
		IIncidentContext GetFieldValue();
	}

	public class ConstantActionField : IIncidentActionField
	{
		public int ActionFieldID { get; set; }
		public string ActionFieldIDString => "{" + ActionFieldID + "}";
		public string NameID { get; set; }

		public Type ContextType { get; set; }

		private IIncidentContext context;

		public ConstantActionField(Type contextType)
		{
			ContextType = contextType;
			ActionFieldID = 0;
			NameID = "{0} - Original Context";
		}
		public bool CalculateField(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			this.context = context;
			return true;
		}

		public IIncidentContext GetFieldValue()
		{
			return context;
		}
	}
}