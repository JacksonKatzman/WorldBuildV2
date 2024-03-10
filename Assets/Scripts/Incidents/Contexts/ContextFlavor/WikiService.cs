using Game.Debug;
using Game.GUI.Adventures;
using Game.Incidents;
using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Game.GUI.Wiki
{
    public class WikiService : SerializedMonoBehaviour
    {
		[SerializeField]
		private WikiTableOfContents tableOfContents;
		[SerializeField]
		private Transform wikiTabSelectorRoot;
		[SerializeField]
		private AllIncidentsWiki allIncidentsWiki;
		[SerializeField]
		private List<IWikiComponent> wikis;
		public WikiTabSelector wikiTabSelectorPrefab;
		public static WikiService Instance { get; private set; }

		private Dictionary<Type, IWikiComponent> wikiDictionary;
		private Dictionary<IWikiComponent, WikiTabSelector> tabSelectors;
		private IWikiComponent currentComponent;
		private bool wikiOpen;

		[Button("Test Load Character Wiki")]
		public void TestLoadCharacterWiki()
        {
			var characters = ContextDictionaryProvider.GetAllContexts<Character>();
			var testPerson = characters.First();
			OpenPage(testPerson.ID.ToString());
		}

		public void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
				EventManager.Instance.AddEventHandler<WorldBuildSimulationCompleteEvent>(OnSimulationComplete);
				EventManager.Instance.AddEventHandler<IsDungeonMasterViewChangedEvent>(OnDungeonMasterViewChanges);
				LoadWikiDictionary();
				HideWikis();
				tableOfContents.Hide();
			}
		}

		public void Fill<T>(T obj)
        {
			var type = obj.GetType();
			if(wikiDictionary.TryGetValue(type, out var wiki))
            {
				wiki.Fill(obj);
            }
        }

		public void OpenWiki()
        {
			if (wikiOpen)
			{
				HideWikis();
				tableOfContents.Hide();
			}
			else
			{
				if (currentComponent != null)
				{
					SwitchToTab(currentComponent);
				}
				else
				{
					if (AdventureService.Instance.IsDungeonMasterView)
					{
						tabSelectors.Values.First().OnClick();
					}
					else
					{
						var selector = tabSelectors.Values.First(x => x.component.FamiliarityRequirement < Enums.ContextFamiliarity.TOTAL);
						selector.OnClick();
					}
				}
			}
        }

		public void OpenPage(string id)
        {
			if(Int32.TryParse(id, out var result))
            {
				var context = ContextDictionaryProvider.AllContexts.GetContextByID(result);
				var contextType = context.GetType();
				if(wikiDictionary.TryGetValue(contextType, out var wiki))
                {
					wiki.Fill(context);
					SwitchToTab(wiki);
                }
			}
        }

		public void SwitchToTab(IWikiComponent component)
        {
			HideWikis();
			currentComponent = component;
			FillTableOfContents(component.GetComponentType());
			component.Show();

			foreach(var pair in tabSelectors)
            {
				pair.Value.button.interactable = true;
            }
			if(tabSelectors.TryGetValue(component, out var tab))
            {
				tab.button.interactable = false;
            }
        }

		public void HideWikis()
        {
			foreach(var pair in wikiDictionary)
            {
				pair.Value.Hide();
            }
        }

		private void FillTableOfContents(Type type)
        {
			if (typeof(IIncidentContext).IsAssignableFrom(type))
			{
				if(AdventureService.Instance.IsDungeonMasterView)
                {
					if (ContextDictionaryProvider.AllContexts.TryGetValue(type, out var allContextsList))
                    {
						tableOfContents.Fill(allContextsList);
					}
					else
                    {
						tableOfContents.Clear();
                    }
				}
				else
                {
					if(AdventureService.Instance.KnownContexts.TryGetValue(type, out var contexts))
                    {
						tableOfContents.Fill(contexts.Keys.ToList());
                    }
					else
					{
						tableOfContents.Clear();
					}
				}
			}
			else
            {
				tableOfContents.Clear();
            }

			tableOfContents.Show();
		}

		private void LoadWikiDictionary()
        {
			wikiDictionary = new Dictionary<Type, IWikiComponent>();
			tabSelectors = new Dictionary<IWikiComponent, WikiTabSelector>();

			foreach(var wiki in wikis)
            {
				var type = wiki.GetType();
				var genericArgs = type.BaseType.GetGenericArguments().ToList();
				var subType = type.BaseType.GetGenericArguments()[0];
				if(!wikiDictionary.ContainsKey(subType))
                {
					wikiDictionary.Add(subType, wiki);
					var selector = Instantiate(wikiTabSelectorPrefab, wikiTabSelectorRoot);
					var tabName = string.Empty;
					if(typeof(IIncidentContext).IsAssignableFrom(subType))
                    {
						tabName = $"{subType.Name}s";
					}
					else if(subType == typeof(List<IncidentReport>))
                    {
						tabName = "World History";
					}
					selector.Setup(wiki, tabName, () => { SwitchToTab(wiki); });
					tabSelectors.Add(wiki, selector);
                }
				else
                {
					OutputLogger.LogError($"Duplicate wiki of type {subType} detected. Multiple wikis of same type are not allowed.");
                }
            }
        }

		private void OnSimulationComplete(WorldBuildSimulationCompleteEvent gameEvent)
        {
			allIncidentsWiki?.Fill(IncidentService.Instance.reports);
			EventManager.Instance.RemoveEventHandler<WorldBuildSimulationCompleteEvent>(OnSimulationComplete);
        }

		private void OnDungeonMasterViewChanges(IsDungeonMasterViewChangedEvent gameEvent)
        {
			if (currentComponent != null)
			{
				if(!gameEvent.isDungeonMasterView)
                {
					currentComponent = wikiDictionary.Values.First(x => x.FamiliarityRequirement < Enums.ContextFamiliarity.TOTAL);
                }
				currentComponent.Clear();
				tabSelectors[currentComponent].OnClick();
				foreach(var selector in tabSelectors.Values)
                {
					selector.gameObject.SetActive(gameEvent.isDungeonMasterView || selector.component.FamiliarityRequirement < Enums.ContextFamiliarity.TOTAL);
                }
			}
        }

		/*Next:
		Make dummy sheets for the rest of the character stuff
		also for factions, races, cities, landmarks, great monsters, items
		handle cases for displaying non context stuff like monsters
		handle swapping between DM view and player view*/
	}
}