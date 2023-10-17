using Game.Data;

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
}
