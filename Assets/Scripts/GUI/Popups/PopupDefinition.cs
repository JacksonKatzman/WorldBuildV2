using Game.Utilities;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.GUI.Popups
{
	[CreateAssetMenu(fileName = nameof(PopupDefinition), menuName = "ScriptableObjects/Popups/" + nameof(PopupDefinition), order = 1)]
	public class PopupDefinition : SerializedScriptableObject
	{
		[SerializeField, ShowInInspector, ReadOnly]
		private int popupType;
		[SerializeField, ValueDropdown("ShowValues"), OnValueChanged("SetPopupType")]
		private string popupTypeString;
		[SerializeField]
		private GameObject prefab;
		private Dictionary<string, int> Constants => GetConstantsDictionary();

		public int PopupType => popupType;
		public GameObject Prefab => prefab;

		private IEnumerable<string> ShowValues()
		{
			return Constants.Keys;
		}

		private void SetPopupType()
		{
			popupType = Constants[popupTypeString];
		}

		private Dictionary<string, int> GetConstantsDictionary()
		{
			var fields = typeof(PopupType).GetFields().Where(c => c.FieldType == typeof(int));
			var dict = new Dictionary<string, int>();
			foreach(var field in fields)
			{
				dict.Add(field.Name, (int)field.GetRawConstantValue());
			}
			return dict;
		}
	}
}
