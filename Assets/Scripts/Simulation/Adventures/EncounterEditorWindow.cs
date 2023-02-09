using Game.Enums;
using Game.Incidents;
using Game.Terrain;
using Game.Utilities;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Simulation
{
	public class EncounterEditorWindow : OdinEditorWindow
	{
        [MenuItem("World Builder/Encounter Editor")]
		private static void OpenWindow()
		{
			var window = GetWindow<EncounterEditorWindow>("Encounter Editor");
			window.minSize = new Vector2(900, 650);
			window.maxSize = new Vector2(1200, 900);
		}

		protected override void OnGUI()
		{
			base.OnGUI();
		}

        private bool ModeChosen { get; set; }
        private string LoadPath => Path.Combine("ScriptableObjects/Encounters/" + savedEncounterName);
        private string SavePath => Path.Combine("ScriptableObjects/Encounters/" + encounterTitle);

        [Button("New Encounter"), HorizontalGroup("B1"), PropertyOrder(-100)]
        public void OnNewButtonPressed()
        {
            components = new ObservableCollection<IAdventureComponent>();
            UpdateModeChosen();
        }

        [Button("Load Encounter"), HorizontalGroup("B1"), PropertyOrder(-100)]
        public void OnLoadButtonPressed()
        {
            var obj = Resources.Load<AdventureEncounterObject>(LoadPath);
            encounterTitle = obj.encounterTitle;
            encounterLocationType = obj.encounterLocationType;
            encounterTypes = obj.encounterTypes;
            allowedBiomes = obj.allowedBiomes;
            contextCriterium = obj.contextCriterium;
            encounterBlurb = obj.encounterBlurb;
            encounterSummary = obj.encounterSummary;
            components = new ObservableCollection<IAdventureComponent>(obj.components);

            UpdateModeChosen();

            OutputLogger.Log("Encounter Loaded!");
        }

        [ValueDropdown("GetSavedEncounters"), HorizontalGroup("B1")]
        public string savedEncounterName;

        [PropertyOrder(-10), ShowIfGroup("ModeChosen")]
        public string encounterTitle;
        [PropertyOrder(-9), ShowIfGroup("ModeChosen")]
        public EncounterLocationType encounterLocationType;

        [ValueDropdown("GetEncounterTypes", IsUniqueList = true, DropdownTitle = "Encounter Types"), PropertyOrder(-8), ShowIfGroup("ModeChosen")]
        public List<EncounterType> encounterTypes;

        [ValueDropdown("GetBiomeTerrainTypes", IsUniqueList = true, DropdownTitle = "Allowed Biomes"), PropertyOrder(-7), ShowIfGroup("ModeChosen")]
        public List<BiomeTerrainType> allowedBiomes;

        [ListDrawerSettings(HideAddButton = true), PropertyOrder(0), ShowIfGroup("ModeChosen")]
        public List<IAdventureContextCriteria> contextCriterium;

        [TextArea(2, 4), PropertyOrder(0), ShowIfGroup("ModeChosen")]
        public string encounterBlurb;

        [TextArea(10, 15), PropertyOrder(0), ShowIfGroup("ModeChosen")]
        public string encounterSummary;

        [ShowInInspector, ShowIfGroup("ModeChosen")]
        public static ObservableCollection<IAdventureComponent> components;

        [Button("Save"), PropertyOrder(10)]
        public void OnSaveButtonPressed()
        {
            var obj = Resources.Load<AdventureEncounterObject>(SavePath);
            if(obj == null)
			{
                obj = CreateInstance<AdventureEncounterObject>();
                AssetDatabase.CreateAsset(obj, "Assets/Resources/" + SavePath + ".asset");
			}
            obj.encounterTitle = encounterTitle;
            obj.encounterLocationType = encounterLocationType;
            obj.encounterTypes = encounterTypes;
            obj.allowedBiomes = allowedBiomes;
            obj.contextCriterium = contextCriterium;
            obj.encounterBlurb = encounterBlurb;
            obj.encounterSummary = encounterSummary;
            obj.components = components.ToList();

            AssetDatabase.SaveAssets();
        }

        public static void UpdateIDs(object sender, NotifyCollectionChangedEventArgs e)
		{
            if(e.NewItems != null)
			{
                OutputLogger.Log("ITEM ADDED");
                var index = 0;
                UpdateComponentIDs(ref index);
			}
            if(e.OldItems != null)
			{
                OutputLogger.Log("ITEM REMOVED");

                var removedItems = e.OldItems.Cast<IAdventureComponent>();
                var removedIds = new List<int>();
                foreach(var item in removedItems)
				{
                    removedIds.AddRange(item.GetRemovedIds());
				}

                var index = 0;
                UpdateComponentIDs(ref index, removedIds);
            }
        }

        private void UpdateModeChosen()
		{
            if(!ModeChosen)
			{
                ModeChosen = true;
                components.CollectionChanged += UpdateIDs;
			}
		}

        private List<string> GetSavedEncounters()
        {
            var files = Directory.GetFiles(Path.Combine(Application.dataPath + SaveUtilities.ENCOUNTER_DATA_PATH), "*.asset").Select(Path.GetFileNameWithoutExtension).ToList();
            return files;
        }

        private IEnumerable<EncounterType> GetEncounterTypes()
        {
            return Enum.GetValues(typeof(EncounterType)).Cast<EncounterType>();
        }

        private IEnumerable<BiomeTerrainType> GetBiomeTerrainTypes()
        {
            return Enum.GetValues(typeof(BiomeTerrainType)).Cast<BiomeTerrainType>();
        }

        private static void UpdateComponentIDs(ref int nextID, List<int> removedIds = null)
        {
            foreach (var component in components)
            {
                component.UpdateComponentID(ref nextID, removedIds);
            }

            OutputLogger.Log("Encounter Component IDs Updated");
        }
    }
}
