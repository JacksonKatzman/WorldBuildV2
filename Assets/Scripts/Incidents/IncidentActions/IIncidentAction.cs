using System;
using Game.Factions;
using UnityEngine;

namespace Game.Incidents
{
	public interface IIncidentAction
	{
		bool VerifyAction(IIncidentContext context, Func<int, IIncidentActionField> delayedAction);
		void PerformAction(IIncidentContext context);
		void UpdateEditor();
	}
}