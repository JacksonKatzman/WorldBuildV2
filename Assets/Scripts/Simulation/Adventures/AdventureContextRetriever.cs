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

		private static readonly Dictionary<string, Func<T, int, string>> replacements = new Dictionary<string, Func<T, int, string>>
		{
			{"LINK", (thing, criteriaID) => thing.Link() }
		};
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

		//lets find a way to not do this roundabout retrieverID shit
		//just have it find the context or whatever, generate the link using .Link() and make a universal linkclickhandler that deals with it like the wiki does
		virtual public void ReplaceTextPlaceholders(ref string text)
		{
			if(string.IsNullOrEmpty(text))
			{
				return;
			}

			HandleTextReplacements(ref text, replacements);

			//For capitalizing after periods
			var postPeriod = new Regex(@"(\. )([a-z])");
			text = postPeriod.Replace(text, m => m.Groups[1].Value + m.Groups[2].Value.ToUpper());
		}

		protected void HandleTextReplacements(ref string text, Dictionary<string, Func<T, int, string>> replacements)
        {
			var matches = Regex.Matches(text, @"{(\w+):(\d+)}");
			foreach (Match match in matches)
			{
				//we have an int and it matches our retriever id
				if (Int32.TryParse(match.Groups[2].Value, out var id) && id == RetrieverID)
				{
					//check dictionary for key
					if (replacements.TryGetValue(match.Groups[1].Value, out var value))
					{
						text = Regex.Replace(text, match.Value, value.Invoke((T)Context, RetrieverID));
					}
				}
			}
		}
	}
}
