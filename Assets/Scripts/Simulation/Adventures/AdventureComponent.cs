﻿using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
	}
}
