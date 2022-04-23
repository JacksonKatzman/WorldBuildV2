using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{
	public class ReplaceModifier : IncidentModifier
	{
		[SerializeField]
		private IncidentModifier replaceWith;

		[SerializeField]
		private int replaceID;
		public ReplaceModifier(List<IIncidentTag> tags, float probability, IncidentModifier replaceWith, int replaceID) : base(tags, probability)
		{
			this.replaceWith = replaceWith;
			this.replaceID = replaceID;
		}

		public override void Setup()
		{
			parent.ReplaceModifier(replaceWith, replaceID);
		}
	}
}