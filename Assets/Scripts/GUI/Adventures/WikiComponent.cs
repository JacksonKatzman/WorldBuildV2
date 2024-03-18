﻿using Game.Enums;
using Game.GUI.Wiki;
using Game.Incidents;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.GUI.Adventures
{
    public abstract class WikiComponent<T> : SerializedMonoBehaviour, IWikiComponent, IPointerClickHandler
    {
        [SerializeField]
        private CanvasGroup mainCanvasGroup;
        [SerializeField]
        private ContextFamiliarity familiarityRequirement;
        protected List<TMP_Text> textFields;
        protected List<IWikiComponent> componentList;

        public CanvasGroup MainCanvasGroup => mainCanvasGroup;
        public ContextFamiliarity FamiliarityRequirement => familiarityRequirement;

        public void Fill(object value)
        {
            Fill((T)value);
        }
        abstract public void Clear();
        virtual public Type GetComponentType()
        {
            return typeof(T);
        }
        protected abstract void Fill(T value);
        private void Awake()
        {
            LoadTextList();
        }

        protected virtual void LoadTextList()
        {
            textFields = new List<TMP_Text>();

            var fields = GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Where(x => x.FieldType == typeof(TMP_Text));
            foreach(var field in fields)
            {
                textFields.Add((TMP_Text)field.GetValue(this));
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            foreach (var textField in textFields)
            {
                var linkIndex = TMP_TextUtilities.FindIntersectingLink(textField, Input.mousePosition, null);
                if (linkIndex >= 0)
                {
                    var linkID = textField.textInfo.linkInfo[linkIndex].GetLinkID();
                    OnLinkClick(linkID);
                    return;
                }
            }
        }

        public void Show()
        {
            Preshow();
            mainCanvasGroup.alpha = 1;
            mainCanvasGroup.interactable = true;
            mainCanvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            mainCanvasGroup.alpha = 0;
            mainCanvasGroup.interactable = false;
            mainCanvasGroup.blocksRaycasts = false;
        }

        virtual public void UpdateByFamiliarity(ContextFamiliarity familiarity)
        {
            
        }

        public void ToggleByFamiliarity(ContextFamiliarity familiarity)
        {
            gameObject.SetActive(familiarity >= FamiliarityRequirement);
        }

        virtual protected void Preshow()
        {

        }

        protected void LoadComponentList()
        {
            componentList = new List<IWikiComponent>();
            var fieldInfos = GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var matchingFieldInfos = fieldInfos.Where(x => typeof(IWikiComponent).IsAssignableFrom(x.FieldType)).ToList();
            foreach (var fieldInfo in matchingFieldInfos)
            {
                var value = (IWikiComponent)fieldInfo.GetValue(this);
                if (value != null)
                {
                    componentList.Add(value);
                }
            }
        }

        protected string Link(IIncidentContext context)
        {
            return string.Format("<u><link=\"{0}\">{1}</link></u>", context.ID, context.Name);
        }

        private void OnLinkClick(string linkID)
        {
            WikiService.Instance.OpenPage(linkID);
        }
    }
}
