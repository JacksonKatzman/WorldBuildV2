using Game.Incidents;
using Game.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.GUI.Wiki
{
	public class IncidentWiki : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField]
		private Transform tabRoot;
		[SerializeField]
		private GameObject wikiTabPrefab;
		[SerializeField]
		private Transform pageRoot;
		[SerializeField]
		private GameObject wikiPagePrefab;

		public Dictionary<int, IncidentWikiPage> pages;
		public List<IncidentWikiTab> tabs;
		private IncidentWikiTab currentTab;
		//private IncidentWikiPage currentPage;

		//private LinkedList<IncidentWikiPage> pageHistory;
		//private LinkedListNode<IncidentWikiPage> currentPage;

		private bool initialized;
		public void Awake()
		{
			//pages = new Dictionary<int, IncidentWikiPage>();
		}

		public void InitializeWiki()
		{
			if(!initialized)
			{
				pages = new Dictionary<int, IncidentWikiPage>();
				tabs = new List<IncidentWikiTab>();
				currentTab = MakeTab();
				tabs.Add(currentTab);
				OpenPage(0);

				initialized = true;
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			var page = currentTab.currentPage;
			var linkIndex = TMP_TextUtilities.FindIntersectingLink(page.Value.wikiText, Input.mousePosition, null);
			if (linkIndex >= 0)
			{
				var linkID = Int32.Parse(page.Value.wikiText.textInfo.linkInfo[linkIndex].GetLinkID());
				if (linkID != page.Value.contextID)
				{
					if(Input.GetKey(KeyCode.LeftControl))
					{
						currentTab = MakeTab();
					}

					OpenPage(linkID);
				}
			}
		}

		public void SwitchToTab(IncidentWikiTab tab)
		{
			currentTab.currentPage.Value.gameObject.SetActive(false);
			currentTab = tab;
			currentTab.currentPage.Value.gameObject.SetActive(true);
		}

		public void GoToPreviousPage()
		{
			currentTab.GoToPreviousPage();
		}

		public void GoToNextPage()
		{
			currentTab.GoToNextPage();
		}

		public void OpenPage(int id, bool newTab = false)
		{
			if(!pages.ContainsKey(id))
			{
				//Get the context in question
				var context = id == 0 ? SimulationManager.Instance.world : SimulationManager.Instance.AllContexts.GetContextByID(id);
				if (context != null)
				{
					//make a new page
					var page = Instantiate(wikiPagePrefab, pageRoot).GetComponent<IncidentWikiPage>();
					page.contextID = id;
					page.wikiTitle.text = context.Name;

					if (id == 0)
					{
						foreach (var item in IncidentService.Instance.reports)
						{
							AddReportToPage(page, item);
						}
					}
					else
					{
						foreach (var item in IncidentService.Instance.reports)
						{
							if (item.Contexts.Values.Contains(context))
							{
								AddReportToPage(page, item);
							}
						}
					}

					pages.Add(id, page);
				}
			}

			currentTab.SwitchToPage(pages[id]);
		}

		private IncidentWikiTab MakeTab()
		{
			var createdTab = Instantiate(wikiTabPrefab, tabRoot).GetComponent<IncidentWikiTab>();
			createdTab.onButtonClicked += SwitchToTab;
			return createdTab;
		}

		private void AddReportToPage(IncidentWikiPage page, IncidentReport report)
		{
			page.wikiText.text += report.ReportYear + ": ";
			page.wikiText.text += report.GenerateLinkedLog();
			page.wikiText.text += "\n";
		}
	}
}
