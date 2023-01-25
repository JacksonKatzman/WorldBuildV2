using Game.Incidents;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.Simulation
{
	[Serializable, HideReferenceObjectPicker]
	public class AdventureContextCriteria<T> : IAdventureContextCriteria where T : IIncidentContext
	{
		public Type ContextType => typeof(T);

		public string ContextID
		{
			get { return contextID; }
			set { contextID = value; }
		}

		public IIncidentContext Context { get; set; }
		public bool IsHistorical => historical;

		[SerializeField, ReadOnly, HorizontalGroup(LabelWidth = 120), PropertyOrder(-1)]
		private string contextTypeName;
		[SerializeField, ReadOnly, HorizontalGroup, PropertyOrder(-1)]
		private string contextID;
		[SerializeField, HorizontalGroup, PropertyOrder(-1)]
		public bool historical;

		public AdventureContextCriteria()
		{
			contextTypeName = ContextType.Name;
		}
	}
}
