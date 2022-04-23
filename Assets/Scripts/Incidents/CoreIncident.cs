using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Game.Incidents
{
	[System.Serializable]
	public class CoreIncident : IIncidentModifierListContainer
	{
		public string incidentName;
		public List<IIncidentTag> tags;
		public int weight;

		public List<IncidentModifier> required;
		public List<IncidentModifier> optional;

		public IncidentContext context;

		public List<IncidentModifier> Incidents => GetIncidentModifiers();

		public CoreIncident() { }

		public CoreIncident(string incidentName, List<IIncidentTag> tags, int weight, List<IncidentModifier> required, List<IncidentModifier> optional)
		{
			this.incidentName = incidentName;
			this.tags = tags;
			this.weight = weight;
			//this.core = core.ShallowCopy(this);
			this.required = new List<IncidentModifier>();
			this.optional = new List<IncidentModifier>();

			required?.ForEach(x => this.required.Add(x.ShallowCopy(this)));
			optional?.ForEach(x => this.optional.Add(x.ShallowCopy(this)));
		}

		public CoreIncident(CoreIncident coreIncident, IncidentContext context) : this(coreIncident.incidentName, coreIncident.tags, coreIncident.weight, coreIncident.required, coreIncident.optional)
		{
			this.context = context;
		}

		public CoreIncident(EditableCoreIncident incidentData) : this(incidentData.incidentName, incidentData.tags, incidentData.weight, incidentData.required, incidentData.optional)
		{
		}

		public bool MatchesCriteria(IncidentContext context)
		{
			return tags.All(x => x.CompareTag(context));
		}

		public void Run()
		{
			var modifiers = new List<IncidentModifier>();
			required.ForEach(x => modifiers.Add(x.ShallowCopy(this)));
			foreach(var modifier in optional)
			{
				if(modifier.MatchesCriteria(context))
				{
					modifiers.Add(modifier.ShallowCopy(this));
				}
			}

			modifiers.ForEach(x => x.Setup());

			modifiers.ForEach(x => x.Run(context));

			modifiers.ForEach(x => x.Finish());

			//Log somehow
		}

		public void Modify(Action<IncidentModifier> action)
		{
			Incidents.ForEach(x => x.Modify(action));
		}

		private List<IncidentModifier> GetIncidentModifiers()
		{
			var list = new List<IncidentModifier>();
			list.AddRange(required);
			list.AddRange(optional);
			return list;
		}

		public void ReplaceModifier(IncidentModifier replaceWith, int replaceID)
		{
			for (int i = 0; i < required.Count; i++)
			{
				if (required[i].replaceID == replaceID)
				{
					required[i] = replaceWith;
				}
			}
			for (int i = 0; i < optional.Count; i++)
			{
				if (optional[i].replaceID == replaceID)
				{
					optional[i] = replaceWith;
				}
			}
		}
	}
}