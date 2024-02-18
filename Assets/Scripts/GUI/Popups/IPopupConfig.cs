using Game.Data;
using System;
using System.Collections.Generic;

namespace Game.GUI.Popups
{
	public interface IPopupConfig 
	{
		public int PopupType { get; }
	}
	abstract public class PopupConfig : IPopupConfig
	{
		abstract public int PopupType { get; }
	}

	public class MonsterInfoCardPopupConfig : PopupConfig
	{
		override public int PopupType => Data.PopupType.MONSTER_INFO_CARD;
		public MonsterData MonsterData { get; set; }
	}

	public class MultiButtonPopupConfig : PopupConfig
    {
		override public int PopupType => Data.PopupType.MULTI_BUTTON;
		public Dictionary<string, Action> ButtonActions { get; set; }
	}
}
