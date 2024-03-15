using Game.GUI.Wiki;
using Game.Incidents;
using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.GUI.Adventures
{
	public class AdventureGuide : SerializedMonoBehaviour
	{
		public Dictionary<Type, GameObject> prefabDictionary;
		public GameObject tableOfContentsLinkPrefab;
		[HideInInspector]
		public AdventureEncounterObject currentEncounter;
		public Transform rootTransform;
		public ScrollRect scrollRect;
		public RectTransform contentPanel;
		public CanvasGroup canvasGroup;
		public CanvasGroup tableOfContentsCanvasGroup;
		public Transform tableOfContentsLinkRoot;

		public TMP_Text adventureTitleText;
		public AdventureTextUIComponent adventureSummaryUI;
		private List<IAdventureUIComponent> uiComponents;
		private List<AdventureComponentUILink> tableOfContents;
		private int numBranches = 0;
		private int numPaths = 0;

		private Action OnEncounterCompleteAction;
		private Action OnEncounterSkippedAction;

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
			//currentEncounter = new Adventure(mainEncounter, sideEncounters);
			//RunEncounter(currentEncounter, null, null);
		}


		public void RunEncounter(AdventureEncounterObject encounter, Action OnEncounterCompleted, Action OnEncounterSkipped)
		{
			currentEncounter = encounter;
			OnEncounterCompleteAction = OnEncounterCompleted;
			OnEncounterSkippedAction = OnEncounterSkipped;

			if (uiComponents == null)
			{
				uiComponents = new List<IAdventureUIComponent>();
			}
			foreach(var component in uiComponents)
            {
				Destroy(component.RectTransform.gameObject);
            }
			uiComponents.Clear();

			if (tableOfContents == null)
			{
				tableOfContents = new List<AdventureComponentUILink>();
			}
			foreach(var link in tableOfContents)
            {
				Destroy(link.gameObject);
            }
			tableOfContents.Clear();

			adventureTitleText.text = encounter.encounterTitle;
			adventureSummaryUI.text.text = encounter.encounterBlurb;
			adventureSummaryUI.text.text += " " + encounter.encounterSummary;
			CreateTableOfContentsEntry(-1, "Summary");

			adventureSummaryUI.ReplaceTextPlaceholders(encounter.contextCriterium);

			foreach (var component in encounter.components)
			{
				if (component.GetType() == typeof(AdventureBranchingComponent))
				{
					var branchingComponent = component as AdventureBranchingComponent;
					numBranches++;
					uiComponents.Add(BuildUIComponent(component));

					foreach (var path in branchingComponent.paths)
					{
						numPaths++;

						foreach (var c in path.components)
						{
							var uiComponent = BuildUIComponent(c, numBranches, numPaths);
							uiComponent.ReplaceTextPlaceholders(encounter.contextCriterium);
							uiComponents.Add(uiComponent);
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

			ToggleCanvasGroup(true);
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
				SnapTo(adventureSummaryUI.RectTransform);
			}
		}

		public void CreateTableOfContentsEntry(int id, string entryText)
		{
			var link = Instantiate(tableOfContentsLinkPrefab, tableOfContentsLinkRoot).GetComponent<AdventureComponentUILink>();
			link.ComponentLinkID = id;
			link.text.text = entryText;
			tableOfContents.Add(link);
		}

		public void ToggleCanvasGroup(bool on)
        {
			canvasGroup.alpha = on ? 1 : 0;
			canvasGroup.interactable = on;
			canvasGroup.blocksRaycasts = on;
        }

		public void ToggleTableOfContents()
		{
			var toggleOn = !tableOfContentsCanvasGroup.interactable;

			tableOfContentsCanvasGroup.alpha = toggleOn ? 1 : 0;
			tableOfContentsCanvasGroup.interactable = toggleOn;
			tableOfContentsCanvasGroup.blocksRaycasts = toggleOn;
		}

		public void OnCurrentEncounterCompleted()
        {
			ToggleCanvasGroup(false);
			OnEncounterCompleteAction?.Invoke();
        }

		public void OnCurrentEncounterFailed()
        {
			ToggleCanvasGroup(false);
			AdventureService.Instance.OnEndAdventure(currentEncounter, false);
		}

		public void OnCurrentEncounterSkipped()
        {
			ToggleCanvasGroup(false);
			OnEncounterSkippedAction?.Invoke();
		}

		public void OnReturnHome()
        {
			ToggleCanvasGroup(false);
			AdventureService.Instance.OnEndAdventure(currentEncounter, false);
		}

		public static bool TryGetContext(int id, out IIncidentContext result)
		{
			//return Instance.currentEncounter.TryGetContext(id, out result);
			return Instance.currentEncounter.TryGetContext(id, out result);
		}

		public static bool TryGetContextCriteria(int id, out IAdventureContextRetriever result)
		{
			return Instance.currentEncounter.TryGetContextCriteria(id, out result);
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
			IAdventureUIComponent uiComponent = (IAdventureUIComponent)instantiatedPrefab.GetComponent(typeof(IAdventureUIComponent));

			uiComponent.ComponentID = adventureComponent.ComponentID;
			uiComponent.BranchGroup = branchGroupID;
			uiComponent.PathGroup = pathGroupID;

			uiComponent.BuildUIComponents(adventureComponent);

			return uiComponent;
		}
	}
}
