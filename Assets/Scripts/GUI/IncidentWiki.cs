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
		private IncidentWikiPage currentPage;
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
				OpenPage(0);

				initialized = true;
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			var linkIndex = TMP_TextUtilities.FindIntersectingLink(currentPage.wikiText, Input.mousePosition, null);
			if (linkIndex >= 0)
			{
				var linkID = Int32.Parse(currentPage.wikiText.textInfo.linkInfo[linkIndex].GetLinkID());
				OpenPage(linkID);
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
				currentPage.gameObject.SetActive(false);
			}

			currentPage = pages[id];
			currentPage.gameObject.SetActive(true);
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
