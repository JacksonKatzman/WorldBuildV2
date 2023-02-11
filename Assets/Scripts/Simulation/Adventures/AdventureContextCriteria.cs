using Game.Incidents;
using Sirenix.OdinInspector;
using System;
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

		public AdventureContextCriteria()
		{
			contextTypeName = ContextType.Name;
		}

		abstract public void RetrieveContext();
		//need a way to ensure contexts for monsters and persons are retrievable
		//once they are we can use them in our text replacements and as links
		//maybe leave the HOW we get people out for now?
		//eventually we will generate a bunch for the area and it can pick from those?
		//just assume we get a random person back for now
		//need a way of getting/parsing the mad lib info
	}
}
