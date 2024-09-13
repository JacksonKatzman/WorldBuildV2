using Game.Enums;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Incidents
{
    public abstract class ContextTrait : SerializedScriptableObject
    { 
        public string traitName;
        public string traitDescription;

        [ValueDropdown("GetAlignments", IsUniqueList = true)]
        public List<CreatureAlignment> compatibleAlignments;

        [JsonIgnore, SerializeField]
        public ObservableCollection<ContextTrait> incompatibleTraits = new ObservableCollection<ContextTrait>();

        public void HookUp()
        {
            incompatibleTraits.CollectionChanged -= OnCollectionChanged;
            incompatibleTraits.CollectionChanged += OnCollectionChanged;
        }

        //Need to find a better way to do this, performance gonna suck on editor
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        static void BeginObservation()
        {
            var assets = AssetDatabase.FindAssets($"t:ContextTrait");
            foreach(var assetString in assets)
            {
                var asset = AssetDatabase.LoadAssetAtPath<ContextTrait>(AssetDatabase.GUIDToAssetPath(assetString));
                asset.HookUp();
            }
        }
#endif

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ContextTrait trait = this;
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    var addedTrait = (ContextTrait)item;
                    if (!addedTrait.incompatibleTraits.Contains(trait))
                    {
                        addedTrait.incompatibleTraits.Add(trait);
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    var removedTrait = (ContextTrait)item;
                    if (removedTrait.incompatibleTraits.Contains(trait))
                    {
                        removedTrait.incompatibleTraits.Remove(trait);
                    }
                }
            }
        }

        private IEnumerable<CreatureAlignment> GetAlignments()
        {
            return Enum.GetValues(typeof(CreatureAlignment)).Cast<CreatureAlignment>();
        }
    }
}