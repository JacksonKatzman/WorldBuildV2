using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GUI.Wiki
{
	public class AdventureGuide : SerializedMonoBehaviour
	{
		public Dictionary<Type, GameObject> prefabDictionary;
		public List<AdventureEncounterObject> encounterObjects;
		public Transform rootTransform;

		private List<IAdventureUIComponent> uiComponents;
		private int numBranches = 0;
		private int numPaths = 0;

		public void Start()
		{
			SetUpAdventure(encounterObjects);
		}

		public void SetUpAdventure(List<AdventureEncounterObject> encounters)
		{
			if(uiComponents == null)
			{
				uiComponents = new List<IAdventureUIComponent>();
			}
			uiComponents.Clear();

			foreach(var encounter in encounters)
			{
				foreach(var component in encounter.components)
				{
					if(component.GetType() == typeof(AdventureBranchingComponent))
					{
						var branchingComponent = component as AdventureBranchingComponent;
						numBranches++;

						foreach(var path in branchingComponent.paths)
						{
							numPaths++;
							foreach(var c in path.components)
							{
								var uic = BuildUIComponent(c, numBranches, numPaths);
								uiComponents.Add(uic);
							}
						}
					}
					else
					{
						var uic = BuildUIComponent(component, numBranches, numPaths);
						uiComponents.Add(uic);
					}
				}
			}
		}

		private IAdventureUIComponent BuildUIComponent(IAdventureComponent adventureComponent, int branchGroupID = -1, int pathGroupID = -1)
		{
			var prefab = prefabDictionary[adventureComponent.GetType()];
			var instantiatedPrefab = Instantiate(prefab, rootTransform);
			IAdventureUIComponent uic = (IAdventureUIComponent)instantiatedPrefab.GetComponent(typeof(IAdventureUIComponent));
			uic.BuildUIComponents(adventureComponent);

			return uic;
		}
	}
}
