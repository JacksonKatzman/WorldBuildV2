using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SimulationManager))]
public class MapViewer : Editor
{
	public override void OnInspectorGUI()
	{
		SimulationManager handler = (SimulationManager)target;

		if(DrawDefaultInspector())
		{
			//handler.DrawNoiseMap();
		}
		/*
		if(GUILayout.Button("Generate"))
		{
			handler.DrawNoiseMap();
		}
		*/
	}
}
