using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GUI.Wiki
{
	public class WikiTab : MonoBehaviour
	{
		public LinkedList<WikiPage> pageHistory;
		public LinkedListNode<WikiPage> currentPage;
		public Action<WikiTab> onButtonClicked;

		public WikiTab()
		{
			pageHistory = new LinkedList<WikiPage>();
		}

		public void SwitchToTab()
		{
			onButtonClicked?.Invoke(this);
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

		public void SwitchToPage(WikiPage incidentWikiPage)
		{
			if (currentPage != null)
			{
				currentPage.Value.gameObject.SetActive(false);
			}

			if (currentPage != null && currentPage.Next != null)
			{
				var last = pageHistory.Last;
				while (last != currentPage)
				{
					last = last.Previous;
					pageHistory.RemoveLast();
				}
			}

			pageHistory.AddLast(incidentWikiPage);
			currentPage = pageHistory.Last;
			currentPage.Value.gameObject.SetActive(true);
		}
	}
}
