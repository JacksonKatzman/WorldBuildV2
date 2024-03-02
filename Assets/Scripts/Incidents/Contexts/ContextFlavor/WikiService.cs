using Game.Debug;
using Game.GUI.Adventures;
using Game.Incidents;
using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.GUI.Wiki
{
    public class WikiService : SerializedMonoBehaviour
    {
		[SerializeField]
		private List<IWikiComponent> wikis;
		public static WikiService Instance { get; private set; }

		private Dictionary<Type, IWikiComponent> wikiDictionary;

		[Button("Test Load Character Wiki")]
		public void TestLoadCharacterWiki()
        {
			var characters = ContextDictionaryProvider.GetAllContexts<Character>();
			var testPerson = characters.First();
			OpenPage(testPerson.ID.ToString());
		}

		public void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
				LoadWikiDictionary();
				HideWikis();
			}
		}

		public void Fill<T>(T obj)
        {
			var type = obj.GetType();
			if(wikiDictionary.TryGetValue(type, out var wiki))
            {
				wiki.Fill(obj);
            }
        }

		public void OpenPage(string id)
        {
			if(Int32.TryParse(id, out var result))
            {
				var context = ContextDictionaryProvider.AllContexts.GetContextByID(result);
				var contextType = context.GetType();
				if(wikiDictionary.TryGetValue(contextType, out var wiki))
                {
					HideWikis();
					wiki.Fill(context);
					wiki.Show();
                }
			}
        }

		public void HideWikis()
        {
			foreach(var pair in wikiDictionary)
            {
				pair.Value.Hide();
            }
        }

		private void LoadWikiDictionary()
        {
			wikiDictionary = new Dictionary<Type, IWikiComponent>();

			foreach(var wiki in wikis)
            {
				var type = wiki.GetType();
				var genericArgs = type.BaseType.GetGenericArguments().ToList();
				var subType = type.BaseType.GetGenericArguments()[0];
				if(!wikiDictionary.ContainsKey(subType))
                {
					wikiDictionary.Add(subType, wiki);
                }
				else
                {
					OutputLogger.LogError($"Duplicate wiki of type {subType} detected. Multiple wikis of same type are not allowed.");
                }
            }
        }
	}
}