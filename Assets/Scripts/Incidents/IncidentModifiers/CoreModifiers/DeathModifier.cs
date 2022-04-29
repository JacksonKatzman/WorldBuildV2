using Game.Creatures;
using Game.Data.EventHandling;
using Game.Data.EventHandling.EventRecording;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{
	public class DeathModifier : IncidentModifier, ITakeableAction, ICreatureContainer
	{
		private IRecordable actionTaker;
		private List<ICreature> targets;

		public IRecordable ActionTaker { set => actionTaker = value; }
		public List<ICreature> Creatures
		{
			get { return targets; }
			set { targets = value; }
		}

		public DeathModifier() : base(new List<IIncidentTag>(), 0)
		{
			targets = new List<ICreature>();
		}
		public DeathModifier(List<IIncidentTag> tags, float probability) : base(tags, probability)
		{
			targets = new List<ICreature>();
		}

		protected override void Init()
		{
			base.Init();
			if (targets == null)
			{
				targets = new List<ICreature>();
			}
		}

		public override void Setup()
		{
		}

		public override void Run(IncidentContext context)
		{
			base.Run(context);
			//for each target call a death event for them
			//will require conversion of personDeathEvent to creatureDeathEvent probably
			//world will need to call it for all people
			//need the age threshold mod
			foreach(var creature in targets)
			{
				var simEvent = new CreatureDiedEvent(creature, "Death");
				EventManager.Instance.Dispatch(simEvent);
			}
		}

		public override void LogModifier()
		{
			targets.ForEach(x => incidentLogs.Add(x.Name + " died."));
		}
	}
}