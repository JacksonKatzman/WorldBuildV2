using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using Game.Factions;

namespace Game.Incidents
{
	[CustomEditor(typeof(LearnAboutReflection))]
	public class LearnAboutReflectionEditor : Editor
	{
		int goofball = 0;
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if (GUILayout.Button("REFLECT!"))
			{
				var faction = new Faction();
				faction.context.Population = 12;
				faction.context.GooPercentage = 40f;
				faction.context.IsFun = true;
				faction.DeployContext();
			}
		}
	}
}