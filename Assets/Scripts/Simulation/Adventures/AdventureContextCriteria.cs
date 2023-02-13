using Game.Incidents;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.Simulation
{
	[Serializable, HideReferenceObjectPicker]
	abstract public class AdventureContextCriteria<T> : IAdventureContextCriteria where T : IIncidentContext
	{
		public Type ContextType => typeof(T);

		public IIncidentContext Context
		{
			get
			{
				if(context == null)
				{
					RetrieveContext();
				}

				return context;
			}
			set
			{
				context = value;
			}
		}
		private IIncidentContext context;
		public bool IsHistorical => historical;

		[SerializeField, ReadOnly, HorizontalGroup(LabelWidth = 120), PropertyOrder(-1)]
		private string contextTypeName;
		[ShowInInspector, HorizontalGroup, PropertyOrder(-1), ReadOnly]
		public int ContextID { get; set; }
		[SerializeField, HorizontalGroup, PropertyOrder(-1)]
		public bool historical;

		abstract public Dictionary<string, Func<T, string>> Replacements { get; }

		public AdventureContextCriteria()
		{
			contextTypeName = ContextType.Name;
		}

		protected T GetTypedContext()
		{
			return (T)Context;
		}

		abstract public void RetrieveContext();
		public void ReplaceTextPlaceholders(ref string text)
		{
			foreach(var pair in Replacements)
			{
				var idReplacementPattern = $"{ContextID}";
				var toReplace = Regex.Replace(pair.Key, "##", idReplacementPattern);
				var replaceWith = pair.Value(GetTypedContext());
				text = text.Replace(@toReplace, replaceWith);
			}
		}
	}
}
