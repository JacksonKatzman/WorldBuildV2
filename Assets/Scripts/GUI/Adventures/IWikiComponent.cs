using System;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public interface IWikiComponent
    {
        public CanvasGroup MainCanvasGroup { get; }
        public void Fill(object obj);
        public void Show();
        public void Hide();
        public Type GetComponentType();
    }
}
