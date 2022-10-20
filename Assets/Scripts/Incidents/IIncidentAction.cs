using System;
using System.Collections.Generic;
using Game.Factions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Incidents
{
	public interface IIncidentAction
	{
		bool VerifyAction(IIncidentContext context, Func<int, IIncidentActionField> delayedAction);
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

		public bool VerifyAction(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			return VerifyContextActionFields(context, delayedCalculateAction);
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
		abstract protected bool VerifyContextActionFields(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction);
	}

	[Serializable]
	public class TestFactionIncidentAction : IncidentAction<FactionContext>
	{
		[HideReferenceObjectPicker]
		public IncidentContextActionField<FactionContext> requiredFaction;

		override public void PerformAction(IIncidentContext context)
		{
			PerformDebugAction();
		}

		protected override bool VerifyContextActionFields(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			var succeeded = requiredFaction.CalculateField(context, delayedCalculateAction);
			return succeeded;
		}

		private void PerformDebugAction()
		{
			OutputLogger.Log("Faction Debug Action Performed!");
		}

		public override void UpdateEditor()
		{
			requiredFaction = new IncidentContextActionField<FactionContext>(ContextType);
			requiredFaction.criteria = new List<IIncidentCriteria>();
		}
	}
}