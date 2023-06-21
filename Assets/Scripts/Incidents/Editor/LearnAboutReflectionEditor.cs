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
using Game.Generators.Names;
using Game.Utilities;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Game.Incidents
{
	[CustomEditor(typeof(LearnAboutReflection))]
	public class LearnAboutReflectionEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if (GUILayout.Button("In game test!"))
			{
				SimulationManager.Instance.DebugRun();
			}

			if (GUILayout.Button("Name Test"))
			{
				var test = (LearnAboutReflection)target;
				test.TestName();
			}

			if (GUILayout.Button("Flavor Test"))
			{
				var phrase = "{1} {1.5] [2} [3] {4} {R:GOOD} {STROMBOLI}";
				GenerateFlavor(phrase);
			}

			if (GUILayout.Button("Thesaurus Test"))
			{
				ThesaurusEntryRetriever.GetSynonyms("mad");
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

		public string GenerateFlavor(string phrase)
		{
			var matches = Regex.Matches(phrase, @"\{[^\n \{\}]+\}");
			foreach(Match match in matches)
			{
				OutputLogger.Log(" %%: " + match.Value);
			}

			matches = Regex.Matches(phrase, @"\{([^\n \{\}]+):(GOOD|EVIL|LAWFUL|CHAOTIC)\}");
			foreach (Match match in matches)
			{
				OutputLogger.Log(" %%: " + match.Value);
			}

			return phrase;
		}
	}

	//[CustomEditor(typeof(NamingThemeCollection))]
	public class NamingThemeCollectionEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Populate Vowels/Consonants"))
			{
				
			}
			base.OnInspectorGUI();
		}
	}
}