using UnityEngine;

namespace Game.Incidents
{
	[CreateAssetMenu(fileName = nameof(FlavorTemplate_CharFact), menuName = "ScriptableObjects/Flavors/" + nameof(FlavorTemplate_CharFact), order = 4)]
	public class FlavorTemplate_CharFact : AbstractFlavorTemplate
	{
		public IndexedObject<Character> primaryCharacter = new IndexedObject<Character>(0);
		public IndexedObject<Faction> primaryFaction = new IndexedObject<Faction>(1);
	}
}