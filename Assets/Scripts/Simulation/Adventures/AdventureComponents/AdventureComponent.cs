using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using System;

namespace Game.Simulation
{
	[HideReferenceObjectPicker]
	public abstract class AdventureComponent : IAdventureComponent
	{
		virtual public bool Completed { get; set; }
		[SerializeField, ReadOnly]
		public int ComponentID { get; set; }
		virtual public void UpdateComponentID(ref int nextID, List<int> removedIds = null)
		{
			ComponentID = nextID;
			nextID++;
		}

		virtual public void UpdateContextIDs(List<int> removedIds = null)
		{

		}

		virtual public List<int> GetRemovedIds()
		{
			return new List<int>() { ComponentID };
		}

		protected void UpdateLinkID(ref int link, List<int> removedIds)
		{
			if (removedIds.Contains(link))
			{
				link = -1;
			}
			else
			{
				var currentLink = link;
				var count = removedIds.Where(x => x < currentLink).Count();
				link -= count;
			}
		}

        public void UpdateStuff(int oldID, int newID)
        {      
			foreach(var field in GetTextFields())
            {
				field.UpdateIDs(oldID, newID);
            }
        }

		private List<AdventureComponentTextField> GetTextFields()
        {
			var fields = GetType().GetFields().Where(x => x.FieldType == typeof(AdventureComponentTextField)).Select(x => x.GetValue(this) as AdventureComponentTextField).ToList();
			return fields;
        }
    }
}
