using UnityEngine;

namespace Game.GUI.Popups
{
	public class Popup : MonoBehaviour
	{
		[SerializeField]
		private RectTransform contentTransform;

		public void UpdateContentScale(float scale)
		{
			contentTransform.localScale = new Vector3(scale, scale, scale);
		}
	}
}
