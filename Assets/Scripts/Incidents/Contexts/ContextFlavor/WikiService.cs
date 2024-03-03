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

		public void OpenPage(string id)
        {
			if(Int32.TryParse(id, out var result))
            {
				var context = ContextDictionaryProvider.AllContexts.GetContextByID(result);
				var contextType = context.GetType();
				if(wikiDictionary.TryGetValue(contextType, out var wiki))
                {
					//HideWikis();
					wiki.Fill(context);
					//wiki.Show();
					//FillTableOfContents(contextType);
					SwitchToTab(wiki);
                }
			}
        }

		public void SwitchToTab(IWikiComponent component)
        {
			HideWikis();
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
				if (ContextDictionaryProvider.AllContexts.TryGetValue(type, out var list))
				{
					tableOfContents.Show();
					tableOfContents.Fill(list);
				}
			}
			else
            {
				tableOfContents.Clear();
            }
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
	}
}