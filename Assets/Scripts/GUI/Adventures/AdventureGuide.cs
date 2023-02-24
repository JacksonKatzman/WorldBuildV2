using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.GUI.Wiki
{
	public class AdventureGuide : SerializedMonoBehaviour
	{
		public Dictionary<Type, GameObject> prefabDictionary;
		public GameObject tableOfContentsLinkPrefab;
		public AdventureEncounterObject mainEncounter;
		public List<AdventureEncounterObject> sideEncounters;
		[HideInInspector]
		public Adventure currentAdventure;
		public Transform rootTransform;
		public ScrollRect scrollRect;
		public RectTransform contentPanel;
		public CanvasGroup tableOfContentsCanvasGroup;
		public Transform tableOfContentsLinkRoot;

		public TMP_Text adventureTitleText;
		public AdventureTextTitlePairUIComponent background;
		private List<IAdventureUIComponent> uiComponents;
		private List<AdventureComponentUILink> tableOfContents;
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
				//SetUpAdventure(encounterObjects);
			}
		}

		[Button("Test Display Adventure")]
		private void TestDisplayAdventure()
		{
			currentAdventure = new Adventure(mainEncounter, sideEncounters);
			SetUpAdventure(currentAdventure);
		}


		public void SetUpAdventure(Adventure adventure)
		{
			if(uiComponents == null)
			{
				uiComponents = new List<IAdventureUIComponent>();
			}
			uiComponents.Clear();

			if(tableOfContents == null)
			{
				tableOfContents = new List<AdventureComponentUILink>();
			}
			tableOfContents.Clear();

			adventureTitleText.text = adventure.mainEncounter.encounterTitle;
			background.text.text = adventure.mainEncounter.encounterBlurb;
			background.text.text += " " + adventure.mainEncounter.encounterSummary;
			CreateTableOfContentsEntry(-1, "Background");

			foreach (var context in mainEncounter.contextCriterium)
			{
				var currentText = background.text.text;
				context.ReplaceTextPlaceholders(ref currentText);
				background.text.text = currentText;
			}

			var encounters = adventure.Encounters;

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
								uic.ReplaceTextPlaceholders(encounter.contextCriterium);
								uiComponents.Add(uic);
							}
						}
					}
					else
					{
						var uic = BuildUIComponent(component, numBranches, numPaths);
						uic.ReplaceTextPlaceholders(encounter.contextCriterium);
						uiComponents.Add(uic);
					}
				}
			}
		}

		public void SetCurrentComponent(int index)
		{
			if (index >= 0)
			{
				var component = uiComponents.First(x => x.ComponentID == index);
				SnapTo(component.RectTransform);
			}
			else
			{
				SnapTo(background.RectTransform);
			}
		}

		public void CreateTableOfContentsEntry(int id, string entryText)
		{
			var link = Instantiate(tableOfContentsLinkPrefab, tableOfContentsLinkRoot).GetComponent<AdventureComponentUILink>();
			link.ComponentLinkID = id;
			link.text.text = entryText;
			tableOfContents.Add(link);
		}

		public void ToggleTableOfContents()
		{
			var toggleOn = !tableOfContentsCanvasGroup.interactable;

			tableOfContentsCanvasGroup.alpha = toggleOn ? 1 : 0;
			tableOfContentsCanvasGroup.interactable = toggleOn;
			tableOfContentsCanvasGroup.blocksRaycasts = toggleOn;
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
