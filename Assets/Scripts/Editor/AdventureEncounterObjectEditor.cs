using UnityEngine;
using UnityEditor;
using Game.Simulation;
using Sirenix.OdinInspector.Editor;

[CustomEditor(typeof(AdventureEncounterObject))]
public class AdventureEncounterObjectEditor : OdinEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(AdventureEncounterObject.Current != target)
        {
            AdventureEncounterObject.Current = (AdventureEncounterObject)target;
        }
    }
}
