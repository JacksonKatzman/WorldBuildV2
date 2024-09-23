using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using System;

namespace Game.Simulation
{
	[HideReferenceObjectPicker, Serializable]
	public abstract class AdventureComponent : IAdventureComponent
	{
		
		virtual public bool Completed { get; set; }

        public void UpdateRetrieverIds(int oldID, int newID)
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
