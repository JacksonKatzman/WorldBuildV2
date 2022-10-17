using System;
using System.Collections.Generic;
using Game.Factions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Incidents
{
	public interface IIncidentAction
	{
		Type ContextType { get; }
		IIncidentContext Context { get; }
		bool VerifyAction(IIncidentContext context);
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

		public bool VerifyAction(IIncidentContext context)
		{
			return VerifyContextActionFields(context);
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
		abstract protected bool VerifyContextActionFields(IIncidentContext context);
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

		protected override bool VerifyContextActionFields(IIncidentContext context)
		{
			var faction = requiredFaction.Value;
			return faction != null;
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