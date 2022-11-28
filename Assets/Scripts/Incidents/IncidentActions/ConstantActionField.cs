using System;

namespace Game.Incidents
{
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
			NameID = ActionFieldIDString + " - Original Context";
		}
		public ConstantActionField(IIncidentContext context) : this(context.ContextType)
		{
			this.context = context;
		}
		public bool CalculateField(IIncidentContext context)
		{
			//this.context = context;
			return true;
		}

		public IIncidentContext GetFieldValue()
		{
			return context;
		}
	}
}