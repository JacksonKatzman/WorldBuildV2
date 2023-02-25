using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.GUI.Wiki
{
	[CreateAssetMenu(fileName = nameof(AdventureKeyword), menuName = "ScriptableObjects/Keywords/" + nameof(AdventureKeyword), order = 1)]
	public class AdventureKeyword : SerializedScriptableObject
	{
		public string keyword;
		[TextArea(5, 10)]
		public string description;
	}
}
