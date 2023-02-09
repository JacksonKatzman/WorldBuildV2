using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.GUI.Wiki
{
	abstract public class Wiki : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField]
		protected Transform tabRoot;
		[SerializeField]
		protected GameObject wikiTabPrefab;
		[SerializeField]
		protected Transform pageRoot;
		[SerializeField]
		protected GameObject wikiPagePrefab;

		public Dictionary<int, WikiPage> pages;
		[HideInInspector]
		public List<WikiTab> tabs;
		protected WikiTab currentTab;

		public void Awake()
		{
			pages = new Dictionary<int, WikiPage>();
			tabs = new List<WikiTab>();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			var page = currentTab.currentPage;
			var linkIndex = TMP_TextUtilities.FindIntersectingLink(page.Value.wikiText, Input.mousePosition, null);
			if (linkIndex >= 0)
			{
				var linkID = page.Value.wikiText.textInfo.linkInfo[linkIndex].GetLinkID();
				OnLinkClick(linkID);
			}
		}

		abstract protected void OnLinkClick(string linkID);
		abstract public void OpenPage(int id);

		public void SwitchToTab(WikiTab tab)
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

		protected WikiTab MakeTab()
		{
			var createdTab = Instantiate(wikiTabPrefab, tabRoot).GetComponent<WikiTab>();
			createdTab.onButtonClicked += SwitchToTab;
			return createdTab;
		}
	}
}
