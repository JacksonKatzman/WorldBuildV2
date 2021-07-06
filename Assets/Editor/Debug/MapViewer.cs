using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldHandler))]
public class MapViewer : Editor
{
	public override void OnInspectorGUI()
	{
		WorldHandler handler = (WorldHandler)target;

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
