using Game.GUI.Wiki;
using HighlightPlus;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.Incidents
{
	public class UserInterfaceService : SerializedMonoBehaviour
	{
		public static UserInterfaceService Instance { get; private set; }
		public IncidentWiki incidentWiki;
		public TMP_Text hexCollectionNameText;

		public void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
				HighlightManager.instance.OnObjectHighlightStart += OnHighlightStart;
			}
		}

		public void Update()
		{
			HandleToggles();
		}


		private void HandleToggles()
		{
			if(Input.GetKeyDown(KeyCode.H))
			{
				if ((HighlightManager.instance.layerMask.value & 1 << LayerMask.NameToLayer("HexTerrain")) > 0)
				{
					HighlightManager.instance.layerMask.value &= ~(1 << LayerMask.NameToLayer("HexTerrain"));
					hexCollectionNameText.enabled = false;
				}
				else
				{
					HighlightManager.instance.layerMask.value |= (1 << LayerMask.NameToLayer("HexTerrain"));
					hexCollectionNameText.enabled = true;
				}
			}
		}
		
		public void ToggleHexCollectionName(bool on, string text)
		{
			hexCollectionNameText.text = text;
		}

		private bool OnHighlightStart(GameObject obj)
		{
			if((HighlightManager.instance.layerMask.value & 1 << LayerMask.NameToLayer("HexTerrain")) > 0)
			{
				obj.GetComponent<HexChunkHighlight>().OnHighlightStart();
			}
			return true;
		}
	}
}