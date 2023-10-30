using Game.Enums;
using Game.Utilities;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	[CreateAssetMenu(fileName = nameof(OrganizationTemplate), menuName = "ScriptableObjects/Data/" + nameof(OrganizationTemplate), order = 1)]
	public class OrganizationTemplate : SerializedScriptableObject
	{
		public string organizationTemplateName;
		public OrganizationType organizationType;
		public int maxPositionsFilledInSimulation;
		public SubOrganization subOrg = new SubOrganization();
	}
}