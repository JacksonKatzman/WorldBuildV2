using Game.Incidents;
using Game.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Wiki
{
	public class IncidentWiki : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField]
		private GameObject wikiPagePrefab;

		public Dictionary<int, IncidentWikiPage> pages;
		//private IncidentWikiPage currentPage;

		private LinkedList<IncidentWikiPage> pageHistory;
		private LinkedListNode<IncidentWikiPage> currentPage;

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
				pageHistory = new LinkedList<IncidentWikiPage>();
				OpenPage(0);

				initialized = true;
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			var linkIndex = TMP_TextUtilities.FindIntersectingLink(currentPage.Value.wikiText, Input.mousePosition, null);
			if (linkIndex >= 0)
			{
				var linkID = Int32.Parse(currentPage.Value.wikiText.textInfo.linkInfo[linkIndex].GetLinkID());
				if (linkID != currentPage.Value.contextID)
				{
					OpenPage(linkID);
				}
			}
		}

		public void GoToPreviousPage()
		{
			if (currentPage.Previous != null)
			{
				currentPage.Value.gameObject.SetActive(false);
				currentPage = currentPage.Previous;
				currentPage.Value.gameObject.SetActive(true);
			}
		}

		public void GoToNextPage()
		{
			if (currentPage.Next != null)
			{
				currentPage.Value.gameObject.SetActive(false);
				currentPage = currentPage.Next;
				currentPage.Value.gameObject.SetActive(true);
			}
		}

		public void OpenPage(int id)
		{
			if(!pages.ContainsKey(id))
			{
				//Get the context in question
				var context = id == 0 ? SimulationManager.Instance.world : SimulationManager.Instance.Contexts.GetContextByID(id);
				if (context != null)
				{
					//make a new page
					var page = Instantiate(wikiPagePrefab, transform).GetComponent<IncidentWikiPage>();
					page.contextID = id;
					page.wikiTitle.text = id + "!";

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

			if(currentPage != null)
			{
				currentPage.Value.gameObject.SetActive(false);
			}

			if(currentPage != null && currentPage.Next != null)
			{
				//pageHistory.Remove(currentPage.Next);
				var last = pageHistory.Last;
				while(last != currentPage)
				{
					last = last.Previous;
					pageHistory.RemoveLast();
				}
			}

			pageHistory.AddLast(pages[id]);
			currentPage = pageHistory.Last;
			currentPage.Value.gameObject.SetActive(true);
		}

		private void AddReportToPage(IncidentWikiPage page, IncidentReport report)
		{
			var textLine = report.ReportLog;
			var matches = Regex.Matches(textLine, @"\{(\d+)\}");

			foreach (Match match in matches)
			{
				var matchString = match.Value;
				var linkedContext = report.Contexts[matchString];
				var linkString = string.Format("<link=\"{0}\">{1}</link>", linkedContext.ID, linkedContext.ID + "!");
				textLine = textLine.Replace(matchString, linkString);
			}

			page.wikiText.text += textLine;
			page.wikiText.text += "\n";
		}

		private void AddPage(int id)
		{
			var page = Instantiate(wikiPagePrefab, transform).GetComponent<IncidentWikiPage>();
			page.contextID = id;

			if(id == 0)
			{

			}
		}
	}
}
