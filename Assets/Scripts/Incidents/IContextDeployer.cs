using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public interface IContextDeployer
	{
		void Deploy();
	}

	public class ContextDeployer : IContextDeployer
	{
		public int delayTime = 0;

		[TypeFilter("GetFilteredTypeList"), OnValueChanged("SetContextType"), LabelText("Incident Type")]
		public IIncidentContext incidentContext;

		public void Deploy()
		{
			if(delayTime == 0)
			{
				IncidentService.Instance.PerformIncidents(incidentContext);
			}
			else
			{
				//add to some timed queue
			}
		}

		private IEnumerable<Type> GetFilteredTypeList()
		{
			var q = typeof(IIncidentContext).Assembly.GetTypes()
				.Where(x => !x.IsAbstract)                                          // Excludes BaseClass
				.Where(x => !x.IsGenericTypeDefinition)                             // Excludes C1<>
				.Where(x => typeof(IIncidentContext).IsAssignableFrom(x));          // Excludes classes not inheriting from BaseClass

			return q;
		}

		void SetContextType()
		{

		}
	}
}