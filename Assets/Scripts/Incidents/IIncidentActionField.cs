using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{
	public interface IIncidentActionField 
	{
		int ActionFieldID { get; set; }
	}

	public enum ActionFieldRetrievalMethod { Criteria, From_Previous, Random };

	[Serializable]
	public class IncidentContextActionField<T> : IIncidentActionField where T : IIncidentContext
	{
		[HideInInspector]
		public Type parentType;

		[OnValueChanged("RetrievalTypeChanged")]
		public ActionFieldRetrievalMethod Method;

		[ShowIf("@this.Method == ActionFieldRetrievalMethod.Criteria && this.ParentTypeMatches")]
		public bool AllowSelf;

		[ShowIf("Method", ActionFieldRetrievalMethod.Criteria), ListDrawerSettings(CustomAddFunction = "AddNewCriteriaItem"), HideReferenceObjectPicker]
		public List<IIncidentCriteria> criteria;

		//Action Field ID
		//When an action is created, all actions have each of their Actionfields reassigned with an ID
		//This is then used to fill in the corresponding values in a report if needed.
		public int ActionFieldID { get; set; }

		[ShowInInspector]
		public string ActionFieldIDString => "{" + ActionFieldID + "}";

		private IIncidentContextProvider value;

		public IIncidentContextProvider Value
		{
			get
			{
				value = RetrieveField();
				return value;
			}
			private set { this.value = value; }
		}

		private bool ParentTypeMatches => parentType == typeof(T);

		public IncidentContextActionField() { }
		public IncidentContextActionField(Type parentType)
		{
			this.parentType = parentType;
		}

		private IIncidentContextProvider RetrieveField()
		{
			OutputLogger.Log("Retrieved Field!");
			return default(T)?.Provider;
		}

		private void AddNewCriteriaItem()
		{
			criteria.Add(new IncidentCriteria(typeof(T)));
		}

		private void RetrievalTypeChanged()
		{
			if (criteria == null)
			{
				criteria = new List<IIncidentCriteria>();
			}
		}
	}
}