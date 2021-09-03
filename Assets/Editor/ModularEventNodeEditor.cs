using System.Collections;
using UnityEngine;
using UnityEditor;
using Game.ModularEvents;
/*
[CustomEditor(typeof(ModularEventBranch))]
public class ModularEventBranchEditor : Editor
{
	public SerializedProperty
		roll_Threshold,
		event_Chance,
		output_EventContext;

	private void OnEnable()
	{
		roll_Threshold = serializedObject.FindProperty("rollThreshold");
		event_Chance = serializedObject.FindProperty("eventChance");
		output_EventContext = serializedObject.FindProperty("outputEventContext");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		//EditorGUILayout.PropertyField(roll_Threshold);
		EditorGUILayout.PropertyField(event_Chance);
		if(event_Chance.floatValue > 0)
		{
			EditorGUILayout.PropertyField(output_EventContext);
		}

		serializedObject.ApplyModifiedProperties();
	}
}
*/