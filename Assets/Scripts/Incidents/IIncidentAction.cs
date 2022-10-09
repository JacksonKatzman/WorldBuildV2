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
		[HideReferenceObjectPicker]
		public IncidentContextActionField<FactionContext> factionCriteria;

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
			factionCriteria = new IncidentContextActionField<FactionContext>(ContextType);
			factionCriteria.criteria = new List<IIncidentCriteria>();
		}
	}
}