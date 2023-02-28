using Game.Utilities;
using UnityEngine;

namespace Game.GUI.Popups
{
	[CreateAssetMenu(fileName = nameof(PopupDefinition), menuName = "ScriptableObjects/Popups/" + nameof(PopupDefinition), order = 1)]
	public class PopupDefinition : ScriptableObject
	{
		[SerializeField, ConstantList(typeof(PopupType))]
		private int popupType;
		[SerializeField]
		private GameObject prefab;
	}
}
