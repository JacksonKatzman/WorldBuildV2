using System;

namespace Game.Incidents
{
	abstract public class ContextualIncidentAction<T> : IncidentAction where T : IIncidentContext
	{
		private IIncidentContext context;
		public Type ContextType => typeof(T);
		public IIncidentContext Context
		{
			get { return context; }
			set
			{
				if (value != null)
				{
					if (value.ContextType == ContextType)
					{
						context = value;
					}
					else
					{
						OutputLogger.LogError(string.Format("Cannot assign context of type {0} to action with context type {1}", value.GetType().ToString(), ContextType.ToString()));
					}
				}
			}
		}
	}
}