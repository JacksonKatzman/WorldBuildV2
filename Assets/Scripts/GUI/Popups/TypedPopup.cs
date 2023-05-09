using System;
using UnityEngine;

namespace Game.GUI.Popups
{
	abstract public class TypedPopup<T> : Popup where T: IPopupConfig
	{
		[SerializeField]
		private RectTransform contentTransform;

		override public Type PopupConfigType => typeof(T);

		public void UpdateContentScale(float scale)
		{
			contentTransform.localScale = new Vector3(scale, scale, scale);
		}
	}
}
