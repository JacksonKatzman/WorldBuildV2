﻿using Game.GUI.Wiki;
using Game.Simulation;
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

		public GameObject generateWorldButton;
		public GameObject wikiButton;
		public GameObject beginAdventureButton;

		public void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
				EventManager.Instance.AddEventHandler<WorldBuildSimulationCompleteEvent>(OnWorldBuildComplete);
				HighlightManager.instance.OnObjectHighlightStart += OnHighlightStart;
				HighlightManager.instance.OnObjectHighlightEnd += OnHighlightEnd;
			}
		}

		public void Update()
		{
			HandleToggles();
		}

		public void OnBeginAdventureButton()
        {
			AdventureService.Instance.BeginAdventure();
			beginAdventureButton.SetActive(false);
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
				//currently hovering over river throws null ref - obj or the get component is null
				obj?.GetComponent<HexChunkHighlight>()?.OnHighlightStart();
			}
			return true;
		}

		private bool OnHighlightEnd(GameObject obj)
        {
			obj?.GetComponent<HexChunkHighlight>()?.OnHighlightEnd();
			return true;
		}

		private void OnWorldBuildComplete(WorldBuildSimulationCompleteEvent gameEvent)
		{
			generateWorldButton.SetActive(false);
			beginAdventureButton.SetActive(true);
			wikiButton.SetActive(true);
		}
	}
}