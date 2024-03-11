using Game.Enums;
using System;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public interface IWikiComponent
    {
        public CanvasGroup MainCanvasGroup { get; }
        public ContextFamiliarity FamiliarityRequirement { get; }
        public void Fill(object obj);
        public void Clear();
        public void Show();
        public void Hide();
        public Type GetComponentType();
        public void UpdateByFamiliarity(ContextFamiliarity familiarity);
        public void ToggleByFamiliarity(ContextFamiliarity familiarity);
    }
}
