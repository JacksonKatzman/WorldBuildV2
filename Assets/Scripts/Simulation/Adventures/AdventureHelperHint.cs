using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Simulation
{
    [CreateAssetMenu(fileName = nameof(AdventureHelperHint), menuName = "ScriptableObjects/Adventures/" + nameof(AdventureHelperHint), order = 10)]
    public class AdventureHelperHint : SerializedScriptableObject
    {
        [TextArea(1,50)]
        public string hint;
    }
}
