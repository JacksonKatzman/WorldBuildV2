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

		public void Start()
		{
			SetUpAdventure(encounterObjects);
		}

		public void SetUpAdventure(List<AdventureEncounterObject> encounters)
		{
			foreach(var encounter in encounters)
			{
				foreach(var component in encounter.components)
				{
					var prefab = prefabDictionary[component.GetType()];
					var instantiatedPrefab = Instantiate(prefab, rootTransform);
					IAdventureUIComponent uic = (IAdventureUIComponent)instantiatedPrefab.GetComponent(typeof(IAdventureUIComponent));
					uic.BuildUIComponents(component);
				}
			}
		}
	}
}
