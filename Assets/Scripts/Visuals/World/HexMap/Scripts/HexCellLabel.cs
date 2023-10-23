using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Terrain
{
	public class HexCellLabel : SerializedMonoBehaviour
	{
		public RectTransform rectTransform;
		public Text hexCellText;
		public Image hexCellHighlight;
		public Dictionary<HexDirection, Image> hexCellBorderImages;

		public void SetText(string text)
		{
			hexCellText.text = text;
		}

		public void SetHighlightColor(Color color)
		{
			hexCellHighlight.color = color;
		}

		public void ToggleHighlight(bool isOn)
		{
			hexCellHighlight.enabled = isOn;
		}

		public void ToggleBorder(HexDirection direction, bool isOn, Color color)
		{
			hexCellBorderImages[direction].enabled = isOn;
			hexCellBorderImages[direction].color = color;
		}
	}
}