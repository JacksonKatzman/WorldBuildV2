using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Incidents
{
    public class RuntimeEditorPrefabs : SerializedMonoBehaviour
    {
		public static RuntimeEditorPrefabs Instance { get; private set; }

		public EditorRuntimeBlock blockPrefab;
		public EditorRuntimeValueDropdown valueDropdownPrefab;
		public EditorRuntimeInput inputPrefab;

		public void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
			}
		}
	}
}