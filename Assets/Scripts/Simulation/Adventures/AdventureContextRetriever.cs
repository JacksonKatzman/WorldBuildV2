using Game.Incidents;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.Simulation
{
	[Serializable, HideReferenceObjectPicker]
	abstract public class AdventureContextRetriever<T> : IAdventureContextRetriever where T : IIncidentContext
	{
		public Type ContextType => typeof(T);
		//now its not turning {1} into linked context
		public List<T> KnownContexts
		{
			get
			{
				if(AdventureService.Instance.KnownContexts.ContainsKey(typeof(T)))
                {
					return AdventureService.Instance.KnownContexts[typeof(T)].Keys.ToList() as List<T>;
				}
				else
                {
					return null;
                }
			}
		}

		[JsonIgnore]
		public IIncidentContext Context
		{
			get
			{
				if(context == null)
				{
					context = RetrieveContext();
				}

				return context;
			}
			set
			{
				context = value;
			}
		}

		[JsonIgnore]
		public T TypedContext => (T)Context;

		private IIncidentContext context;
		public bool IsHistorical => historical;

		[SerializeField, ReadOnly, HorizontalGroup(LabelWidth = 120), PropertyOrder(-1)]
		private string contextTypeName;
		[SerializeField, HorizontalGroup, PropertyOrder(-1), ReadOnly]
		public int RetrieverID { get; set; }
		[SerializeField, HorizontalGroup, PropertyOrder(-1)]
		public bool historical;

		abstract public Dictionary<string, Func<T, int, string>> Replacements { get; }
		abstract public void SpawnPopup();

		public AdventureContextRetriever()
		{
			contextTypeName = ContextType.Name;
		}

		protected T GetTypedContext()
		{
			return (T)Context;
		}

		abstract public T RetrieveContext();
		public void ReplaceTextPlaceholders(ref string text)
		{
			if(string.IsNullOrEmpty(text))
			{
				return;
			}

			foreach(var pair in Replacements)
			{
				var idReplacementPattern = $"{RetrieverID}";
				var toReplace = Regex.Replace(pair.Key, "##", idReplacementPattern);
				var replaceWith = pair.Value(GetTypedContext(), RetrieverID);
				text = text.Replace(@toReplace, replaceWith);
			}

			//For capitalizing after periods
			var postPeriod = new Regex(@"(\. )([a-z])");
			text = postPeriod.Replace(text, m => m.Groups[1].Value + m.Groups[2].Value.ToUpper());
		}
	}
}
