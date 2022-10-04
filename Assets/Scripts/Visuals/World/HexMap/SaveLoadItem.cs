using TMPro;
using UnityEngine;

namespace Game.Visuals.Hex
{
	public class SaveLoadItem : MonoBehaviour
	{
		public SaveLoadMenu menu;

		[SerializeField]
		TMP_Text nameText;

		public string MapName
		{
			get
			{
				return mapName;
			}
			set
			{
				mapName = value;
				nameText.text = value;
			}
		}

		string mapName;

		public void Select()
		{
			menu.SelectItem(mapName);
		}
	}
}
