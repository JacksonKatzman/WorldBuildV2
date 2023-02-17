using Game.Simulation;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GUI.Wiki
{
	public interface IAdventureUIComponent
	{
		public int ComponentID { get; set; }
		public int BranchGroup { get; set; }
		public int PathGroup { get; set; }
		public RectTransform RectTransform { get; }
		public void BuildUIComponents(IAdventureComponent component);
		public void ReplaceTextPlaceholders(List<IAdventureContextCriteria> contexts);
	}
}
