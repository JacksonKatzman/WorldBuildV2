﻿using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public interface IIncidentActionField 
	{
		int ActionFieldID { get; set; }
		string ActionFieldIDString { get; }
		string NameID { get; set; }

		Type ContextType { get; }

		IIncidentContextProvider RetrieveField(IIncidentContext context);
	}

	public class ActionFieldInfo
	{
	}

	public enum ActionFieldRetrievalMethod { Criteria, From_Previous, Random };

	[Serializable]
	public class IncidentContextActionField<T> : IIncidentActionField where T : IIncidentContext
	{
		[HideInInspector]
		public Type parentType;

		[OnValueChanged("RetrievalTypeChanged")]
		public ActionFieldRetrievalMethod Method;

		[ShowIf("@this.ParentTypeMatches"), HideIf("@this.Method == ActionFieldRetrievalMethod.From_Previous")]
		public bool AllowSelf;

		[ShowIf("Method", ActionFieldRetrievalMethod.Criteria), ListDrawerSettings(CustomAddFunction = "AddNewCriteriaItem"), HideReferenceObjectPicker]
		public List<IIncidentCriteria> criteria;

		[ShowIf("Method", ActionFieldRetrievalMethod.From_Previous), ValueDropdown("GetActionFieldIdentifiers"), OnValueChanged("SetPreviousFieldID")]
		public string previousField;

		private int previousFieldID = -1;

		//Action Field ID
		//When an action is created, all actions have each of their Actionfields reassigned with an ID
		//This is then used to fill in the corresponding values in a report if needed.
		//NEXT STEPS: Pass along each IDString/Value pair to the IncidentReport dictionary along with
		//a string that uses them and the IDs for the spawner and the report itself
		public int ActionFieldID { get; set; }

		public string NameID { get; set; }

		[ShowInInspector]
		public string ActionFieldIDString => "{" + ActionFieldID + "}";

		public Type ContextType => typeof(T);
		private bool ParentTypeMatches => parentType == typeof(T);

		public IncidentContextActionField() { }
		public IncidentContextActionField(Type parentType)
		{
			this.parentType = parentType;
			RetrievalTypeChanged();
		}

		public IIncidentContextProvider RetrieveField(IIncidentContext context)
		{
			if(Method == ActionFieldRetrievalMethod.Criteria)
			{
				return RetrieveFieldByCriteria(context);
			}
			else if(Method == ActionFieldRetrievalMethod.From_Previous)
			{
				return RetrieveFieldFromPrevious(context);
			}
			else
			{
				if (AllowSelf)
				{
					return SimulationManager.Instance.Providers[typeof(T)].First();
				}
				else
				{
					return SimulationManager.Instance.Providers[typeof(T)].Where(x => x.GetContext() != context).ToList().First();
				}
			}
		}

		private IIncidentContextProvider RetrieveFieldByCriteria(IIncidentContext context)
		{
			var criteriaContainer = new IncidentCriteriaContainer(criteria);
			List<IIncidentContextProvider> possibleMatches;
			if (AllowSelf)
			{
				possibleMatches = SimulationManager.Instance.Providers[typeof(T)].Where(x => criteriaContainer.Evaluate(x.GetContext()) == true).ToList();
			}
			else
			{
				possibleMatches = SimulationManager.Instance.Providers[typeof(T)].Where(x => x.GetContext() != context && criteriaContainer.Evaluate(x.GetContext()) == true).ToList();
			}
			//change this to get at random using some static utility fn
			return possibleMatches.Count > 0 ? possibleMatches.First() : null;
		}

		private IIncidentContextProvider RetrieveFieldFromPrevious(IIncidentContext context)
		{
			return default(T)?.Provider;
		}

		private void RetrievalTypeChanged()
		{
			if (criteria == null)
			{
				criteria = new List<IIncidentCriteria>();
			}
		}

		private void AddNewCriteriaItem()
		{
			criteria.Add(new IncidentCriteria(typeof(T)));
		}

		private List<string> GetActionFieldIdentifiers()
		{
			var ids = new List<string>();
			var matches = IncidentEditorWindow.actionFields.Where(x => x.ContextType == ContextType && x != this).ToList();
			matches.ForEach(x => ids.Add(x.NameID));
			return ids;
		}

		private void SetPreviousFieldID()
		{
			previousFieldID = IncidentEditorWindow.actionFields.Find(x => x.NameID == previousField).ActionFieldID;
		}
	}
}