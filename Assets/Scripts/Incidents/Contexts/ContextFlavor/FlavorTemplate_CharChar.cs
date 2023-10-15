using UnityEngine;

namespace Game.Incidents
{
	[CreateAssetMenu(fileName = nameof(FlavorTemplate_CharChar), menuName = "ScriptableObjects/Flavors/" + nameof(FlavorTemplate_CharChar), order = 2)]
	public class FlavorTemplate_CharChar : AbstractFlavorTemplate
	{
		public IndexedObject<Character> primaryCharacter = new IndexedObject<Character>(0);
		public IndexedObject<Character> secondaryCharacter = new IndexedObject<Character>(1);
	}
}