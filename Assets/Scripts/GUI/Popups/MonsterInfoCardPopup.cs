using Game.Creatures;
using System.Collections.Generic;

namespace Game.GUI.Popups
{
	public class MonsterInfoCardPopup : TypedPopup<MonsterInfoCardPopupConfig>
	{
		protected override bool CompareTo(MonsterInfoCardPopupConfig config)
		{
			return this.config.MonsterData == config.MonsterData;
		}

		protected override void Setup()
		{

		}
	}
}
