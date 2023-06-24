using Game.Enums;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Game.Incidents
{
	public class FlavorEditorWindow : OdinEditorWindow
	{
        public static FlavorEditorWindow Instance = null;

        private Dictionary<FlavorType, Func<string>> FlavorValues = new Dictionary<FlavorType, Func<string>>()
        { 
            { FlavorType.SYNONYM, () => Instance.synonymKey },
            { FlavorType.REASON, () => Instance.alignment.ToString() }
        };

        [MenuItem("World Builder/Flavor Editor")]
        public static void OpenWindow()
		{
            var window = GetWindow<FlavorEditorWindow>("FlavorEditor");
            window.minSize = new Vector2(300, 650);
            window.maxSize = new Vector2(550, 900);
            AssetDatabase.Refresh();
            //window.position = new Rect(new Vector2(posX, posY), new Vector2(900, 650));
        }

        private string GS()
		{
            return synonymKey;
		}

        public FlavorEditorWindow()
		{
            Instance = this;
		}

        protected override void OnGUI()
        {
            base.OnGUI();
        }

        [ValueDropdown("GetFlavorTypes")]
        public FlavorType flavorType;
        
        [ShowIf("@this.ShouldShowSynonymKeyDropdown"), ValueDropdown("GetSynonymKeys")]
        public string synonymKey;

        [Button("Add New Synonym Entry", ButtonSizes.Medium), ShowIf("@this.ShouldShowNewSynonymEntryButton")]
        public void OpenNewEntryPopup()
		{
            NewThesaurusEntryWindow.OpenWindow();
		}

        [ShowIf("ShouldShowAlignment")]
        public CreatureAlignment alignment;

        [Button("Copy To Clipboard", ButtonSizes.Medium), ShowIf("ShouldShowCopyToClipboardButton")]
        public void CopyToClipboard()
        {
            var bfs = BuildFlavorString();
            GUIUtility.systemCopyBuffer = BuildFlavorString();
        }

        private string BuildFlavorString()
		{
            var flavorString = "{" + flavorType.ToString();
            if(FlavorValues.TryGetValue(flavorType, out var value))
			{
                flavorString += (":" + value.Invoke());
			}
            flavorString += "}";

            return Regex.Replace(flavorString, @"\s", string.Empty);
        }

        private IEnumerable<FlavorType> GetFlavorTypes()
		{
            return Enum.GetValues(typeof(FlavorType)).Cast<FlavorType>();
        }

        private IEnumerable<string> GetSynonymKeys()
		{
            var list = new List<string>() { "-" };
            list.AddRange(ThesaurusProvider.Thesaurus.Keys.ToList());
            return list;
		}

        private IEnumerable<CreatureAlignment> GetAlignments()
        {
            return Enum.GetValues(typeof(CreatureAlignment)).Cast<CreatureAlignment>();
        }

        private bool ShouldShowSynonymKeyDropdown => flavorType == FlavorType.SYNONYM;
        private bool ShouldShowNewSynonymEntryButton => ShouldShowSynonymKeyDropdown && synonymKey == "-";
        private bool ShouldShowCopyToClipboardButton()
        {
            return (flavorType == FlavorType.SYNONYM && synonymKey != "-") ||
                (flavorType == FlavorType.REASON);
        }

        private bool ShouldShowAlignment()
		{
            return flavorType == FlavorType.REASON;
		}
    }

	public class NewThesaurusEntryWindow : OdinEditorWindow
	{
        public string entry;
        [ShowIf("@this.newEntries.Count > 0")]
        public List<NewThesaurusEntry> newEntries = new List<NewThesaurusEntry>();

        [Button(ButtonSizes.Medium)]
        public void AddEntry()
		{
            if(ThesaurusProvider.Thesaurus.ContainsKey(entry.ToUpper()))
            {
                OutputLogger.LogError($"Thesaurus already contains entries for {entry}");
                return;
            }

            var root = ThesaurusEntryRetriever.GetSynonyms(entry);

            if (root == null || root.Containers.Count == 0)
			{
                OutputLogger.LogError($"{entry} did not return any valid synonyms.");
                return;
			}

            foreach(var container in root.Containers)
			{
                newEntries.Add(new NewThesaurusEntry(entry, container.Entry, RemoveEntry));
			}
		}

        public static void OpenWindow()
        {
            var window = GetWindow<NewThesaurusEntryWindow>("NewThesaurusEntryWindow");
            window.minSize = new Vector2(300, 450);
            window.maxSize = new Vector2(550, 700);
        }

        private void RemoveEntry(NewThesaurusEntry entry)
		{
            newEntries.Remove(entry);
		}
    }
}