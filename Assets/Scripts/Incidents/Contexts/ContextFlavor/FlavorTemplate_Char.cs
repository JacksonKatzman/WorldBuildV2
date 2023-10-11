using UnityEngine;

namespace Game.Incidents
{
	[CreateAssetMenu(fileName = nameof(FlavorTemplate_Char), menuName = "ScriptableObjects/Flavors/" + nameof(FlavorTemplate_Char), order = 3)]
	public class FlavorTemplate_Char : AbstractFlavorTemplate
	{
		public IndexedObject<Character> primaryCharacter = new IndexedObject<Character>(0);
	}
}