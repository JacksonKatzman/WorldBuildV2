using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public enum ActionFieldRetrievalMethod { Criteria, From_Previous, Random };

	[Serializable, HideReferenceObjectPicker]
	public class ContextualIncidentActionField<T> : IIncidentActionField where T : IIncidentContext
	{
		[HideInInspector]
		public Type parentType;

		[OnValueChanged("RetrievalTypeChanged"), PropertyOrder(-1), ShowInInspector, ShowIf("ShowMethodChoice")]
		virtual public ActionFieldRetrievalMethod Method { get; set; }

		[ShowInInspector, ShowIf("@this.ShowAllowSelf")]
		public bool AllowSelf;

		[ShowInInspector]
		public bool AllowNull;

		[ShowIf("@this.ShowStandardCriteria"), ListDrawerSettings(CustomAddFunction = "AddNewCriteriaItem"), HideReferenceObjectPicker]
		public List<IncidentActionFieldCriteria> criteria;

		[ShowIf("Method", ActionFieldRetrievalMethod.From_Previous), ValueDropdown("GetActionFieldIdentifiers"), OnValueChanged("SetPreviousFieldID")]
		public string previousField;

		[HideInInspector]
		public int previousFieldID = -1;

		public int ActionFieldID { get; set; }

		public string NameID { get; set; }

		[ShowInInspector]
		virtual public string ActionFieldIDString => ActionFieldID == 0 ? "None" : "{" + ActionFieldID + "}";

		public Type ContextType => typeof(T);
		private bool ParentTypeMatches => parentType == typeof(T);
		virtual protected bool ShowMethodChoice => true;
		virtual protected bool ShowAllowSelf => ParentTypeMatches && ShowMethodChoice && Method != ActionFieldRetrievalMethod.From_Previous;
		virtual protected bool ShowStandardCriteria => Method == ActionFieldRetrievalMethod.Criteria;

		public IIncidentContext value;
		public IIncidentActionField delayedValue;

		public ContextualIncidentActionField() 
		{
			RetrievalTypeChanged();
		}
		public ContextualIncidentActionField(Type parentType) : this()
		{
			this.parentType = parentType;
		}

		virtual public IIncidentContext GetFieldValue()
		{
			if (Method == ActionFieldRetrievalMethod.From_Previous)
			{
				return delayedValue == null? null : delayedValue.GetFieldValue();
			}
			else
			{
				return value;
			}
		}

		virtual public T GetTypedFieldValue()
		{
			if (Method == ActionFieldRetrievalMethod.From_Previous)
			{
				return delayedValue == null ? default(T) :(T)delayedValue.GetFieldValue();
			}
			else
			{
				return value == null? default(T) : (T)value;
			}
		}

		virtual public bool CalculateField(IIncidentContext context)
		{
			if(Method == ActionFieldRetrievalMethod.Criteria)
			{
				value = RetrieveFieldByCriteria(context);
			}
			else if(Method == ActionFieldRetrievalMethod.From_Previous)
			{
				delayedValue = RetrieveFieldFromPrevious(context);
			}
			else
			{
				value = RetrieveFieldAtRandom(context);
			}

			var status = AllowNull ? true : ((value != null) || (delayedValue != null));
			return status;
		}

		virtual protected IIncidentContext RetrieveFieldByCriteria(IIncidentContext context)
		{
			var criteriaContainer = new IncidentActionFieldCriteriaContainer(criteria);
			List<IIncidentContext> possibleMatches;
			if (AllowSelf)
			{
				possibleMatches = SimulationManager.Instance.Contexts[typeof(T)].Where(x => criteriaContainer.Evaluate(x, context) == true).ToList();
			}
			else
			{
				possibleMatches = SimulationManager.Instance.Contexts[typeof(T)].Where(x => x != context && criteriaContainer.Evaluate(x, context) == true).ToList();
			}

			return possibleMatches.Count > 0 ? SimRandom.RandomEntryFromList(possibleMatches) : null;
		}

		virtual protected IIncidentContext RetrieveFieldAtRandom(IIncidentContext context)
		{
			if (AllowSelf)
			{
				var possibleValues = SimulationManager.Instance.Contexts[typeof(T)];
				return SimRandom.RandomEntryFromList(possibleValues);
			}
			else
			{
				var possibleValues = SimulationManager.Instance.Contexts[typeof(T)].Where(x => x != context).ToList();
				return SimRandom.RandomEntryFromList(possibleValues);
			}
		}

		protected IIncidentActionField RetrieveFieldFromPrevious(IIncidentContext context)
		{
			return IncidentService.Instance.CurrentIncident.ActionContainer.GetContextFromActionFields(previousFieldID);
			//return delayedCalculateAction.Invoke(previousFieldID);
		}

		private void RetrievalTypeChanged()
		{
			if (criteria == null)
			{
				criteria = new List<IncidentActionFieldCriteria>();
			}
		}

		private void AddNewCriteriaItem()
		{
			criteria.Add(new IncidentActionFieldCriteria(typeof(T)));
		}

		virtual protected List<string> GetActionFieldIdentifiers()
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