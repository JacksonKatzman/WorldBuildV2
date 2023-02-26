using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GUI.Wiki
{
	public class Tooltip : MonoBehaviour
	{
		public TMP_Text header;
		public TMP_Text content;
		public LayoutElement layoutElement;
		public RectTransform rectTransform;

		public int characterWrapLimit;

		private void Update()
		{
			layoutElement.enabled = (header.text.Length > characterWrapLimit || content.text.Length > characterWrapLimit) ? true : false;

			Vector2 mPos = Input.mousePosition;
			float pivotX = mPos.x / Screen.width;
			float pivotY = mPos.y / Screen.height;

			rectTransform.pivot = new Vector2(pivotX, pivotY);

			transform.position = mPos;
		}
	}
}
