using Game.Enums;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Game.Data.EventHandling.EventRecording;
using System.Linq;

namespace Game.ModularEvents
{
	[CreateAssetMenu(fileName = nameof(ModularEventNode), menuName = "ScriptableObjects/ModularEvents/" + nameof(ModularEventNode), order = 1)]
	public class ModularEventNode : ScriptableObject
	{
		[Title("Event Input Context", "Determines the kind of context that can trigger this event.")]
		public EventContextContainer inputContext;
		[Space, Title("Branch Style", "Determines how this event will choose its branches.")]
		public EventBranchStyle branchStyle;

		[SerializeReference, ShowIf("branchStyle", EventBranchStyle.PRIORITY), Space, Title("Event Output Context", "Context Type to pair with Priorities when choosing subsequent events.")]
		public IEventContext outputContext;

		[ShowIf("branchStyle", EventBranchStyle.ROLL), Space, Title("Stat Requirement")]
		public StatType statType;

		[ShowIf("branchStyle", EventBranchStyle.ROLL), Space, Title("Possible Event Outcomes:")]
		public List<ModularEventBranch> branches;
	}
}