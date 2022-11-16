using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using Game.Factions;
using System.Reflection;
using System.Collections.Generic;
using Game.Simulation;
using System.Data;
using System.IO;

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

			if (GUILayout.Button("DataTable test"))
			{
				var table1 = new DataTable();
				table1.Columns.Add(new DataColumn("ContextID"));
				table1.Columns.Add(new DataColumn("ContextType"));
				table1.Columns.Add(new DataColumn("Property"));
				table1.Columns.Add(new DataColumn("0"));
				table1.Columns.Add(new DataColumn("1"));
				table1.Rows.Add("Faction", "0", "Influence", "10", "11");

				var table2 = new DataTable();
				table2.Columns.Add(new DataColumn("ContextID"));
				table2.Columns.Add(new DataColumn("ContextType"));
				table2.Columns.Add(new DataColumn("Property"));
				table2.Columns.Add(new DataColumn("1"));
				table2.Columns.Add(new DataColumn("2"));
				table2.Columns.Add(new DataColumn("3"));
				table2.Rows.Add("Faction", "1", "Influence", "6", "11", "16");

				var table3 = new DataTable();
				table3.Columns.Add(new DataColumn("ContextID"));
				table3.Columns.Add(new DataColumn("ContextType"));
				table3.Columns.Add(new DataColumn("Property"));
				table3.Columns.Add(new DataColumn("0"));
				table3.Columns.Add(new DataColumn("6"));
				table3.Columns.Add(new DataColumn("7"));
				table3.Rows.Add("Faction", "2", "Influence", "7", "13", "19");

				table1.Merge(table2);
				table1.Merge(table3);
				OutputLogger.Log("Merge complete!");

				table1.ToCSV(Application.dataPath + "/Resources/" + "testCSV" + ".csv");
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