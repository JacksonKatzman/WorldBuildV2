using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using Game.Factions;
using System.Reflection;
using System.Collections.Generic;
using Game.Simulation;

namespace Game.Incidents
{
	[CustomEditor(typeof(LearnAboutReflection))]
	public class LearnAboutReflectionEditor : Editor
	{
		int goofball = 0;
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if (GUILayout.Button("In game test!"))
			{
				SimulationManager.Instance.DebugRun();
			}
		}
		public static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(Type openGenericType, Assembly assembly)
		{
			return from x in assembly.GetTypes()
				   from z in x.GetInterfaces()
				   let y = x.BaseType
				   where
				   (y != null && y.IsGenericType &&
				   openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition())) ||
				   (z.IsGenericType &&
				   openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
				   select x;
		}
	}
}