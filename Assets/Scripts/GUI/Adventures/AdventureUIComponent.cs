using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.GUI.Wiki
{
	public abstract class AdventureUIComponent<T> : SerializedMonoBehaviour, IAdventureUIComponent, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler where T : IAdventureComponent
	{
		protected static byte FULL_ALPHA = 255;
		protected static byte FADED_ALPHA = 100;

		public static float DOUBLE_CLICK_THRESHOLD = 0.2f;
		private float lastClickTime = -1;

		private static LTDescr delay;
		protected bool hovered;

		public int ComponentID { get; set; }
		public int BranchGroup { get; set; }
		public int PathGroup { get; set; }
		public RectTransform RectTransform => GetComponent<RectTransform>();

		abstract protected List<TMP_Text> AssociatedTexts { get; }
		virtual public bool Completed { get; set; }
		public void ToggleCompleted()
		{
			Completed = !Completed;
			ToggleElements();
		}
		
		public void OnPointerClick()
		{
			OutputLogger.Log("CLICK!");
			if(Time.time - lastClickTime < DOUBLE_CLICK_THRESHOLD)
			{
				OutputLogger.Log("DOUBLE CLICK!");
				ToggleCompleted();
			}

			lastClickTime = Time.time;
		}

		protected Color32 SwapColorAlpha(Color32 color, byte alpha)
		{
			return new Color32(color.r, color.g, color.b, alpha);
		}

		public void BuildUIComponents(IAdventureComponent component)
		{
			BuildUIComponents((T)component);
		}
		abstract public void BuildUIComponents(T component);
		virtual public void ReplaceTextPlaceholders(List<IAdventureContextCriteria> contexts)
		{
			foreach (var text in AssociatedTexts)
			{
				ReplaceTextPlaceholders(contexts, text);
			}
		}

		virtual public void OnPointerClick(PointerEventData eventData)
		{
			foreach (var text in AssociatedTexts)
			{
				HandleClicks(text);
			}
		}
		virtual protected void ToggleElements()
		{
			foreach(var text in AssociatedTexts)
			{
				ToggleText(text);
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			hovered = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			hovered = false;
			TooltipService.HideTooltip();
		}

		protected void ReplaceTextPlaceholders(List<IAdventureContextCriteria> contexts, TMP_Text text)
		{
			if (string.IsNullOrEmpty(text.text))
			{
				return;
			}

			foreach (var context in contexts)
			{
				var currentText = text.text;
				context.ReplaceTextPlaceholders(ref currentText);
				text.text = currentText;
			}

			AddKeywordLinks(text);
		}

		protected void AddKeywordLinks(TMP_Text text)
		{
			var keywordRegex = new Regex(@"\[([A-Z_]+)\]");
			text.text = keywordRegex.Replace(text.text, m => string.Format("<link={0}><u><b>{0}</u></b></link>", m.Groups[1].Value));
		}

		protected void ToggleText(TMP_Text text)
		{
			text.color = Completed ? SwapColorAlpha(text.color, FADED_ALPHA) : SwapColorAlpha(text.color, FULL_ALPHA);
		}

		protected void HandleClicks(TMP_Text text)
		{
			var linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, null);
			if (linkIndex >= 0)
			{
				var linkID = text.textInfo.linkInfo[linkIndex].GetLinkID();
				if(InfoService.Keywords.TryGetValue(linkID, out var value))
				{
					OutputLogger.Log("* Opening Popup for: " + value.keyword);
				}
				else if(Int32.TryParse(linkID, out var result))
				{
					if(AdventureGuide.TryGetContextCriteria(result, out var context))
					{
						context.SpawnPopup();
					}
				}
				else
				{
					OutputLogger.LogWarning("* No Keyword found for: " + linkID);
				}
			}
		}

		protected void HandleTooltips(TMP_Text text)
		{
			var linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, null);
			if (linkIndex >= 0)
			{
				var linkID = text.textInfo.linkInfo[linkIndex].GetLinkID();
				if(int.TryParse(linkID, out int result))
				{
					TooltipService.ShowTooltip("Context with ID: " + result);
				}
				else
				{
					TooltipService.HideTooltip();
				}
			}
			else
			{
				TooltipService.HideTooltip();
			}
		}
	}
}
