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
/*
	public class AdventureUIBranchingComponent : IAdventureUIComponent
	{
		public List<AdventureUIPathComponent> paths;
		public void BuildUIComponents(IAdventureComponent component, Transform root, Dictionary<Type, GameObject> prefabDictionary)
		{
			paths = new List<AdventureUIPathComponent>();

			var branchingComponent = component as AdventureBranchingComponent;
			foreach(var path in branchingComponent.paths)
			{
				var pathComponent = new AdventureUIPathComponent();
				pathComponent.BuildUIComponents(path, root, prefabDictionary);
				paths.Add(pathComponent);
			}
		}
	}

	public class AdventureUIPathComponent : IAdventureUIComponent
	{
		public List<AdventureUIComponent> components;
		public void BuildUIComponents(IAdventureComponent component, Transform root, Dictionary<Type, GameObject> prefabDictionary)
		{
			
		}
	}
*/
}
