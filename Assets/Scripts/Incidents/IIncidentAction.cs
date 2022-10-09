using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Incidents
{
	public interface IIncidentAction
	{
		Type ContextType { get; }
		IIncidentContext Context { get; }
		void PerformAction(IIncidentContext context);

		void UpdateEditor();
	}

	abstract public class IncidentAction<T> : IIncidentAction where T : IIncidentContext
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

		virtual public void PerformAction(IIncidentContext context)
		{
			PerformDebugAction();
		}

		private void PerformDebugAction()
		{
			OutputLogger.Log("Debug Action Performed!");
		}

		abstract public void UpdateEditor();
	}

	[Serializable]
	public class TestFactionIncidentAction : IncidentAction<FactionContext>
	{
		[ActionField, HideReferenceObjectPicker]
		public IncidentActionField<FactionContext> factionCriteria;

		override public void PerformAction(IIncidentContext context)
		{
			var faction = factionCriteria.Value;
			PerformDebugAction();
		}

		private void PerformDebugAction()
		{
			OutputLogger.Log("Faction Debug Action Performed!");
		}

		public override void UpdateEditor()
		{
			factionCriteria = new IncidentActionField<FactionContext>(ContextType);
			factionCriteria.criteria = new List<IIncidentCriteria>();
		}
	}


	// -------------------------------------

	[AttributeUsage(AttributeTargets.Field)]
	public class ActionField : Attribute { }

	public enum ActionFieldRetrievalMethod { Criteria, From_Previous, Random };

	public interface IIncidentActionField { }

	[Serializable]
	public class IncidentActionField<T> : IIncidentActionField
	{
		[HideInInspector]
		public Type parentType;

		[OnValueChanged("RetrievalTypeChanged")]
		public ActionFieldRetrievalMethod Method;

		[ShowIf("@this.Method == ActionFieldRetrievalMethod.Criteria && this.ParentTypeMatches")]
		public bool AllowSelf;

		[ShowIf("Method", ActionFieldRetrievalMethod.Criteria), ListDrawerSettings(CustomAddFunction = "AddNewCriteriaItem"), HideReferenceObjectPicker]
		public List<IIncidentCriteria> criteria;

		private T value;

		public T Value
		{
			get 
			{ 
				value = RetrieveField();
				return value;
			}
			private set { this.value = value; }
		}

		private bool ParentTypeMatches => parentType == typeof(T);

		public IncidentActionField() { }
		public IncidentActionField(Type parentType)
		{
			this.parentType = parentType;
		}

		private T RetrieveField()
		{
			OutputLogger.Log("Retrieved Field!");
			return default(T);
		}

		private void AddNewCriteriaItem()
		{
			criteria.Add(new IncidentCriteria(typeof(T)));
		}

		private void RetrievalTypeChanged()
		{
			if(criteria == null)
			{
				criteria = new List<IIncidentCriteria>();
			}
		}
	}

}