using Game.Simulation;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GUI.Adventures
{
	public interface IAdventureUIComponent
	{
		public RectTransform RectTransform { get; }
		public void BuildUIComponents(IAdventureComponent component);
		public void ReplaceTextPlaceholders(List<IAdventureContextRetriever> contexts);
	}
}
