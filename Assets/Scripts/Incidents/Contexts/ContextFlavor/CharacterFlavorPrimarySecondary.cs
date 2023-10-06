using UnityEngine;

namespace Game.Incidents
{
	[CreateAssetMenu(fileName = nameof(CharacterFlavorPrimarySecondary), menuName = "ScriptableObjects/Flavors/" + nameof(CharacterFlavorPrimarySecondary), order = 2)]
	public class CharacterFlavorPrimarySecondary : AbstractFlavorTemplate
	{
		public IndexedObject<Character> primaryCharacter = new IndexedObject<Character>(0);
		public IndexedObject<Character> secondaryCharacter = new IndexedObject<Character>(1);
	}
}