using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GUI.Wiki
{
	public class AdventureGuide : SerializedMonoBehaviour
	{
		public Dictionary<Type, GameObject> prefabDictionary;
		public List<AdventureEncounterObject> encounterObjects;
		public Transform rootTransform;
		public ScrollRect scrollRect;
		public RectTransform contentPanel;

		private List<IAdventureUIComponent> uiComponents;
		private int numBranches = 0;
		private int numPaths = 0;

		public static AdventureGuide Instance { get; private set; }
		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
				SetUpAdventure(encounterObjects);
			}
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
						uiComponents.Add(BuildUIComponent(component));

						foreach (var path in branchingComponent.paths)
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

		public void SetCurrentComponent(int index)
		{
			var component = uiComponents.First(x => x.ComponentID == index);
			SnapTo(component.RectTransform);
		}

		private void SnapTo(RectTransform target)
		{
			Canvas.ForceUpdateCanvases();

			var contentPos = (Vector2)scrollRect.transform.InverseTransformPoint(scrollRect.content.position);
			var childPos = (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
			var endPos = contentPos - childPos;

			if (!scrollRect.horizontal)
			{
				endPos.x = scrollRect.content.anchoredPosition.x;
			}

			if (!scrollRect.vertical)
			{
				endPos.y = scrollRect.content.anchoredPosition.y;
			}
			scrollRect.content.anchoredPosition = endPos;
		}

		private IAdventureUIComponent BuildUIComponent(IAdventureComponent adventureComponent, int branchGroupID = -1, int pathGroupID = -1)
		{
			var prefab = prefabDictionary[adventureComponent.GetType()];
			var instantiatedPrefab = Instantiate(prefab, rootTransform);
			IAdventureUIComponent uic = (IAdventureUIComponent)instantiatedPrefab.GetComponent(typeof(IAdventureUIComponent));

			uic.ComponentID = adventureComponent.ComponentID;
			uic.BranchGroup = branchGroupID;
			uic.PathGroup = pathGroupID;

			uic.BuildUIComponents(adventureComponent);

			return uic;
		}
	}
}
