using Game.Debug;
using Game.Incidents;
using Game.Simulation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatTracker
{
	List<int> deathAges;
	public StatTracker()
	{
		deathAges = new List<int>();
		EventManager.Instance.AddEventHandler<RemoveContextEvent>(StoreDeathAge);
	}

	private void StoreDeathAge(RemoveContextEvent gameEvent)
	{
		if(gameEvent.contextType == typeof(Character))
		{
			var character = gameEvent.context as Character;
			deathAges.Add(character.Age);
		}
	}

	public void ReportDeathAges()
	{
		var pre40 = (float)deathAges.Where(x => x < 40).Count();
		var pre60 = (float)deathAges.Where(x => x < 60).Count();
		var total = (float)deathAges.Count;

		OutputLogger.Log("----- Death Age Data -----");
		OutputLogger.Log($"Total Deaths: {total}");
		OutputLogger.Log($"Percent Before 40: {(pre40 / total) * 100f}");
		OutputLogger.Log($"Percent Before 60: {(pre60 / total) * 100f}");
	}
}
