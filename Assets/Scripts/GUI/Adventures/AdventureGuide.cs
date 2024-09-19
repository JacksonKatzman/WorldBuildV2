using Game.Debug;
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
		
		[SerializeField]
		private AdventureSectionUIComponent AdventureSectionUIPrefab;
		[SerializeField]
		private AdventureEncounterObject testEncounter;
		public GameObject tableOfContentsLinkPrefab;
		[HideInInspector]
		public AdventureEncounterObject currentEncounter;
		public Transform rootTransform;
		public ScrollRect scrollRect;
		public CanvasGroup canvasGroup;
		public CanvasGroup tableOfContentsCanvasGroup;
		public Transform tableOfContentsLinkRoot;

		private Dictionary<AdventureSection, AdventureSectionUIComponent> uiComponents;
		private List<AdventureSectionUILink> tableOfContents;
		private List<AdventureSection> adventureSections;
		private AdventureSection currentSection;

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
			RunEncounter(testEncounter, null, null);
		}


		public void RunEncounter(AdventureEncounterObject encounter, Action OnEncounterCompleted, Action OnEncounterSkipped)
		{
			currentEncounter = encounter;
			OnEncounterCompleteAction = OnEncounterCompleted;
			OnEncounterSkippedAction = OnEncounterSkipped;

			if (uiComponents == null)
			{
				uiComponents = new Dictionary<AdventureSection, AdventureSectionUIComponent>();
			}

			foreach(var pair in uiComponents)
            {
				Destroy(pair.Value.gameObject);
            }
			uiComponents.Clear();

			if (tableOfContents == null)
			{
				tableOfContents = new List<AdventureSectionUILink>();
			}
			foreach(var link in tableOfContents)
            {
				Destroy(link.gameObject);
            }
			tableOfContents.Clear();

			adventureSections = new List<AdventureSection>();
			var summarySection = CreateSummarySection(encounter.encounterTitle, encounter.encounterSummary, encounter.sections.First());
			adventureSections.Add(summarySection);
			adventureSections.AddRange(encounter.sections);

			BeginSection(adventureSections.First());
			ToggleCanvasGroup(true);
		}

		public void BeginSection(AdventureSection section)
        {
			currentSection = section;

			if(uiComponents.TryGetValue(section, out var uiComponent))
            {
				HideAllSections();
				uiComponent.ToggleCanvasGroup(true);
            }
			else
            {
				HideAllSections();
				var sectionUI = BuildSectionUI(section);
				uiComponents.Add(section, sectionUI);
				CreateTableOfContentsEntry(section);
            }
        }

		private AdventureSectionUIComponent BuildSectionUI(AdventureSection section)
        {
			var instantiatedPrefab = Instantiate(AdventureSectionUIPrefab, rootTransform);
			var uiComponent = instantiatedPrefab.GetComponent<AdventureSectionUIComponent>();
			uiComponent.CreateSectionUI(section);
			return uiComponent;
		}

		private void HideAllSections()
        {
			foreach(var section in uiComponents.Values)
            {
				section.ToggleCanvasGroup(false);
            }
        }

		private AdventureSection CreateSummarySection(string title, string summary, AdventureSection firstSection)
        {
			var summarySection = new AdventureSection("Summary", firstSection);
			summarySection.components.Add(new AdventureTitleComponent(title));
			summarySection.components.Add(new AdventureTextComponent(summary));
			return summarySection;
		}

		public void CreateTableOfContentsEntry(AdventureSection section)
		{
			var link = Instantiate(tableOfContentsLinkPrefab, tableOfContentsLinkRoot).GetComponent<AdventureSectionUILink>();
			link.Setup(section);
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
			AdventureService.Instance.OnEndAdventure(false);
		}

		public void OnCurrentEncounterSkipped()
        {
			ToggleCanvasGroup(false);
			OnEncounterSkippedAction?.Invoke();
		}

		public void OnReturnHome()
        {
			ToggleCanvasGroup(false);
			AdventureService.Instance.OnEndAdventure(false);
		}

		public static bool TryGetContext(int id, out IIncidentContext result)
		{
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
	}
}
