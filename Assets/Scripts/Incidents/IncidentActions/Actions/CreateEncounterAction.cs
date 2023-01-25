using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public class CreateEncounterAction : GenericIncidentAction
	{
		[SerializeField, OnValueChanged("OnEncounterChanged")]
		public AdventureEncounter encounter;

		[ListDrawerSettings(HideAddButton = true, HideRemoveButton = true)]
		public List<IncidentActionFieldContainer> actionFields;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			var copy = UnityEngine.Object.Instantiate(encounter);
			var historicals = copy.contextCriterium.Where(x => x.IsHistorical).ToList();
			for(int i = 0; i < historicals.Count; i++)
			{
				historicals[i].Context = actionFields[i].actionField.GetFieldValue();
			}

			AdventureService.Instance.AddAvailableEncounter(copy);
		}

		private void OnEncounterChanged()
		{
			actionFields = new List<IncidentActionFieldContainer>();
			foreach(var criteria in encounter.contextCriterium)
			{
				if(criteria.IsHistorical)
				{
					var container = new IncidentActionFieldContainer();
					container.ForceSetContextType(criteria.ContextType);
					actionFields.Add(container);
				}
			}

			IncidentEditorWindow.UpdateActionFieldIDs();
		}
	}
}