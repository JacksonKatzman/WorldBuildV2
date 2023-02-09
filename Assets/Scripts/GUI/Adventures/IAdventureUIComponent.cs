using Game.Simulation;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GUI.Wiki
{
	public interface IAdventureUIComponent
	{
		public int BranchGroup { get; set; }
		public int PathGroup { get; set; }
		public void BuildUIComponents(IAdventureComponent component);
	}
}
