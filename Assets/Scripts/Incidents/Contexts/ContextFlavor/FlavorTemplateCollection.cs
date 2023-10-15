using Game.Enums;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	[CreateAssetMenu(fileName = nameof(FlavorTemplateCollection), menuName = "ScriptableObjects/Flavors/" + nameof(FlavorTemplateCollection), order = 1)]
	public class FlavorTemplateCollection : SerializedScriptableObject
	{
		[ValueDropdown("GetFilteredTypeList")]
		public Type objectType;

		public List<AbstractFlavorTemplate> flavorTemplates;

		public string resourcePath;

		public List<AbstractFlavorTemplate> GetMatches(OrganizationType permType, int goodEvilValue, int lawfulChaoticValue)
		{
			return flavorTemplates.Where(x => x.MeetsRequirements(permType, goodEvilValue, lawfulChaoticValue)).ToList();
		}

		[Button("Compile Object List")]
		private void CompileObjectList()
		{
			var resources = Resources.LoadAll("ScriptableObjects/Flavor/" + resourcePath, objectType);
			foreach (var resource in resources)
			{
				var typedResource = resource as AbstractFlavorTemplate;
				if (flavorTemplates == null)
				{
					flavorTemplates = new List<AbstractFlavorTemplate>();
				}

				if (!flavorTemplates.Contains(typedResource))
				{
					flavorTemplates.Add(typedResource);
				}
			}
		}

		private IEnumerable<Type> GetFilteredTypeList()
		{
			var q = typeof(IIncidentContext).Assembly.GetTypes()
				.Where(x => !x.IsAbstract)                                          // Excludes BaseClass
				.Where(x => !x.IsGenericTypeDefinition)                             // Excludes Generics
				.Where(x => typeof(AbstractFlavorTemplate).IsAssignableFrom(x));

			return q;
		}
	}
}