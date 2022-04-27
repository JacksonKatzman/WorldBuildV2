using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public abstract class IncidentModifier : IIncidentModifierListContainer, IModifierInfoContainer
	{
		[SerializeField]
		private List<IIncidentTag> tags;
		[SerializeField]
		private float probability;

		[HideInInspector]
		public IIncidentModifierListContainer parent;

		[SerializeField]
		private List<IncidentModifier> required;
		[SerializeField]
		private List<IncidentModifier> optional;

		public int replaceID;

		[HideInInspector]
		public List<string> incidentLogs;

		public List<IncidentModifier> Incidents => GetIncidentModifiers();

		protected IncidentModifier(List<IIncidentTag> tags, float probability)
		{
			this.tags = tags;
			this.probability = probability;
			required = new List<IncidentModifier>();
			optional = new List<IncidentModifier>();
		}

		protected IncidentModifier(List<IIncidentTag> tags, float probability, List<IncidentModifier> required, List<IncidentModifier> optional)
		{
			this.tags = tags;
			this.probability = probability;
			this.required = required;
			this.optional = optional;
		}

		protected void ProvideModifierInfo(Action<IModifierInfoContainer> action)
		{
			parent.Modify(action);
		}

		public IncidentModifier ShallowCopy(IIncidentModifierListContainer parent)
		{
			var modifier = (IncidentModifier)this.MemberwiseClone();
			modifier.parent = parent;
			modifier.Init();
			return modifier;
		}

		protected virtual void Init()
		{

		}

		public bool MatchesCriteria(IncidentContext context)
		{
			return SimRandom.RandomFloat01() <= probability && tags.All(x => x.CompareTag(context));
		}

		public virtual void Setup() { }
		public virtual void Finish() { }
		public virtual void Run(IncidentContext context)
		{
			var modifiers = new List<IncidentModifier>();
			required.ForEach(x => modifiers.Add(x.ShallowCopy(this)));
			foreach (var modifier in optional)
			{
				if (modifier.MatchesCriteria(context))
				{
					modifiers.Add(modifier.ShallowCopy(this));
				}
			}

			modifiers.ForEach(x => x.Setup());

			modifiers.ForEach(x => x.Run(context));

			modifiers.ForEach(x => x.Finish());

			incidentLogs = new List<string>();
			LogModifier();
			modifiers.ForEach(x => incidentLogs.AddRange(x.incidentLogs));
		}

		protected virtual void LogModifier()
		{
		}

		public virtual void Modify(Action<IModifierInfoContainer> action)
		{
			action.Invoke(this);
			Incidents.ForEach(x => x.Modify(action));
		}

		private List<IncidentModifier> GetIncidentModifiers()
		{
			var list = new List<IncidentModifier>();
			if (required != null)
			{
				list.AddRange(required);
			}
			if (optional != null)
			{
				list.AddRange(optional);
			}
			return list;
		}

		public void ReplaceModifier(IncidentModifier replaceWith, int replaceID)
		{
			for(int i = 0; i < required.Count; i++)
			{
				if(required[i].replaceID == replaceID)
				{
					required[i] = replaceWith;
				}
			}
			for (int i = 0; i < optional.Count; i++)
			{
				if (optional[i].replaceID == replaceID)
				{
					optional[i] = replaceWith;
				}
			}
		}
	}

	public class NullIncidentModifier : IncidentModifier
	{
		public NullIncidentModifier() : base(new List<IIncidentTag>(), 0)
		{

		}
		public NullIncidentModifier(List<IIncidentTag> tags, float probability) : base(tags, probability)
		{
		}
	}
}