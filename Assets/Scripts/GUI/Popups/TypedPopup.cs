using System;
using UnityEngine;

namespace Game.GUI.Popups
{
	abstract public class TypedPopup<T> : Popup where T: IPopupConfig
	{
		[SerializeField]
		private RectTransform contentTransform;

		override public Type PopupConfigType => typeof(T);

		protected T config;

		public void UpdateContentScale(float scale)
		{
			contentTransform.localScale = new Vector3(scale, scale, scale);
		}

		public override void Setup(IPopupConfig config)
		{
			this.config = (T)config;
			Setup();
		}

		abstract protected void Setup();

		public override bool CompareTo(IPopupConfig config)
		{
			if(config.GetType() == PopupConfigType)
			{
				return CompareTo((T)config);
			}

			return false;
		}
		abstract protected bool CompareTo(T config);
	}
}
