using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Game.Data.EventHandling.EventRecording;
using Game.Enums;

namespace Game.ModularEvents
{
	public class ModularEventManager
	{
		List<ModularEventNode> nodes => DataManager.Instance.modularEventNodes;

		public void CallPersonEvent(Person person)
		{
			var matches = nodes.Where(n => n.inputContext.contextObject == null).ToList();
			var randomNode = SimRandom.RandomEntryFromList(matches);

			if (randomNode != null)
			{
				var record = new EventRecord();
				RunEvent(randomNode, null, person, record);
				SimulationManager.Instance.eventRecorder.eventRecords.Add(record);
			}
		}

		private void RunEvent(ModularEventNode node, IEventContext eventContext, Person person, EventRecord record)
		{
			if (eventContext != null)
			{
				eventContext.RunEvent(person, record);
			}
			if (node.branchStyle == EventBranchStyle.PRIORITY)
			{
				var prioList = person.priorities.SortedList();
				for (int i = 0; i < prioList.Count; i++)
				{
					var possibleNodes = nodes.Where(n => n.inputContext.contextType == node.outputContext.GetType() && n.inputContext.priorityType == prioList[i]).ToList();
					if (possibleNodes.Count > 0)
					{
						var chosenNode = SimRandom.RandomEntryFromList(possibleNodes);
						RunEvent(chosenNode, node.outputContext, person, record);
					}
				}
			}
			else if (node.branchStyle == EventBranchStyle.ROLL)
			{
				var branches = node.branches.OrderBy(o => o.rollThreshold).ToList();
				var roll = eventContext.GetRollMargin(person, node.statType);
				for (int i = 0; i < branches.Count; i++)
				{
					if (roll >= branches[i].rollThreshold || i == branches.Count - 1)
					{
						branches[i].RunBranch(person, record);
						var randomFloat = SimRandom.RandomFloat01();
						if (branches[i].hasFollowUpEvent && randomFloat > branches[i].eventChance)
						{
							var possibleNodes = nodes.Where(n => n.inputContext.contextType == branches[i].outputEventContext.contextType && n.inputContext.priorityType == branches[i].outputEventContext.priorityType).ToList();
							if (possibleNodes.Count > 0)
							{
								var chosenNode = SimRandom.RandomEntryFromList(possibleNodes);
								RunEvent(chosenNode, node.outputContext, person, record);
							}
						}
					}
				}
			}
		}
	}
}