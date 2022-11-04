using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public enum ActionFieldRetrievalMethod { Criteria, From_Previous, Random };

	[Serializable]
	public class ContextualIncidentActionField<T> : IIncidentActionField where T : IIncidentContext
	{
		[HideInInspector]
		public Type parentType;

		[OnValueChanged("RetrievalTypeChanged"), ShowIf("@this.ShowMethodChoice"), PropertyOrder(-1)]
		virtual public ActionFieldRetrievalMethod Method { get; set; }

		[ShowIf("@this.ShowAllowSelf")]
		public bool AllowSelf;

		[ShowIf("Method", ActionFieldRetrievalMethod.Criteria), ListDrawerSettings(CustomAddFunction = "AddNewCriteriaItem"), HideReferenceObjectPicker]
		public List<IIncidentCriteria> criteria;

		[ShowIf("Method", ActionFieldRetrievalMethod.From_Previous), ValueDropdown("GetActionFieldIdentifiers"), OnValueChanged("SetPreviousFieldID")]
		public string previousField;

		[HideInInspector]
		public int previousFieldID = -1;

		public int ActionFieldID { get; set; }

		public string NameID { get; set; }

		[ShowInInspector]
		virtual public string ActionFieldIDString => "{" + ActionFieldID + "}";

		public Type ContextType => typeof(T);
		private bool ParentTypeMatches => parentType == typeof(T);
		virtual protected bool ShowMethodChoice => true;
		virtual protected bool ShowAllowSelf => ParentTypeMatches && ShowMethodChoice && Method != ActionFieldRetrievalMethod.From_Previous;

		protected IIncidentContext value;
		private IIncidentActionField delayedValue;

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
				return delayedValue.GetFieldValue();
			}
			else
			{
				return value;
			}
		}

		virtual public bool CalculateField(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			if(Method == ActionFieldRetrievalMethod.Criteria)
			{
				value = RetrieveFieldByCriteria(context);
			}
			else if(Method == ActionFieldRetrievalMethod.From_Previous)
			{
				delayedValue = RetrieveFieldFromPrevious(context, delayedCalculateAction);
			}
			else
			{
				if (AllowSelf)
				{
					var possibleValues = SimulationManager.Instance.Contexts[typeof(T)];
					value = SimRandom.RandomEntryFromList(possibleValues);
				}
				else
				{
					var possibleValues = SimulationManager.Instance.Contexts[typeof(T)].Where(x => x != context).ToList();
					value = SimRandom.RandomEntryFromList(possibleValues);
				}
			}

			return (value != null) || (delayedValue != null);
		}

		private IIncidentContext RetrieveFieldByCriteria(IIncidentContext context)
		{
			var criteriaContainer = new IncidentCriteriaContainer(criteria);
			List<IIncidentContext> possibleMatches;
			if (AllowSelf)
			{
				possibleMatches = SimulationManager.Instance.Contexts[typeof(T)].Where(x => criteriaContainer.Evaluate(x) == true).ToList();
			}
			else
			{
				possibleMatches = SimulationManager.Instance.Contexts[typeof(T)].Where(x => x != context && criteriaContainer.Evaluate(x) == true).ToList();
			}

			return possibleMatches.Count > 0 ? SimRandom.RandomEntryFromList(possibleMatches) : null;
		}

		private IIncidentActionField RetrieveFieldFromPrevious(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			return delayedCalculateAction.Invoke(previousFieldID);
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