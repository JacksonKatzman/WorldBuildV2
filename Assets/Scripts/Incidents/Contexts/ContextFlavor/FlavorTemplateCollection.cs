using Game.Enums;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	[CreateAssetMenu(fileName = nameof(FlavorTemplateCollection), menuName = "ScriptableObjects/Flavors/" + nameof(FlavorTemplateCollection), order = 1)]
	public class FlavorTemplateCollection : SerializedScriptableObject
	{
		public List<AbstractFlavorTemplate> flavorTemplates;

		public List<AbstractFlavorTemplate> GetMatches(OrganizationType permType, int goodEvilValue, int lawfulChaoticValue)
		{
			return flavorTemplates.Where(x => x.MeetsRequirements(permType, goodEvilValue, lawfulChaoticValue)).ToList();
		}
	}
}