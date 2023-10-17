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
using System.Xml.Linq;
using Game.Terrain;
using Cysharp.Threading.Tasks;

namespace Game.Incidents
{
	[CustomEditor(typeof(LearnAboutReflection))]
	public class LearnAboutReflectionEditor : Editor
	{
		private bool loadingOn = false;
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if (GUILayout.Button("In game test!"))
			{
				SimulationManager.Instance.AsyncRun();
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

			if (GUILayout.Button("Recursion Test"))
			{
				var randomCharacter = SimRandom.RandomEntryFromList(SimulationManager.Instance.world.People);
				var family = CharacterExtensions.GetExtendedFamily(randomCharacter);
				if (family.Count > 0)
				{
					var member = SimRandom.RandomEntryFromList(family);
					var contexts = new Dictionary<string, IIncidentContext>();
					contexts.Add("{0}", randomCharacter);
					contexts.Add("{1}", member);
					var title = StaticFlavorCollections.HandleCharacterRelationshipTitles("{RELATE:0/1}", contexts);
					OutputLogger.Log(title);
				}
			}

			if(GUILayout.Button("Test Loading Screen"))
			{
				loadingOn = !loadingOn;
				if (loadingOn)
				{
					EventManager.Instance.Dispatch(new ShowLoadingScreenEvent("Testing Loading!"));
				}
				else
				{
					EventManager.Instance.Dispatch(new HideLoadingScreenEvent());
				}
			}

			if (GUILayout.Button("Center Camera"))
			{
				HexMapCamera.CenterPosition();
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

		public void XMLTest()
		{
			XElement word = XElement.Parse(@"<p><ent>Abandon</ent><br/>
<hw>A*bandon </hw> <pr> (oops)</pr>, <pos>v. t.</pos> <vmorph>[<pos>imp. oops2; p. p.</pos> <conjf>Abandoned</conjf> <pr>(-doops3;nd)</pr>; <pos>p. pr. oops4; vb. n.</pos> <conjf>Abandoning</conjf>.]</vmorph> <ety>[OF. <ets>abandoner</ets>, F. <ets>abandonner</ets>; <ets>a</ets> (L. <ets>ad</ets>) + <ets>bandon</ets> permission, authority, LL. <ets>bandum</ets>, <ets>bannum</ets>, public proclamation, interdiction, <ets>bannire</ets> to proclaim, summon: of Germanic origin; cf. Goth. <ets>bandwjan</ets> to show by signs, to designate OHG. <ets>ban</ets> proclamation. The word meant to proclaim, put under a ban, put under control; hence, as in OE., to compel, subject, or to leave in the control of another, and hence, to give up. See <er>Ban</er>.]</ety> <sn>1.</sn> <def>To cast or drive out; to banish; to expel; to reject.</def> <mark>[Obs.]</mark><br/>
		  [<source> 1913 Webster </source>] </p> ");

			List<XElement> stuff = word.Elements("ent").ToList();
			OutputLogger.Log(stuff[0].ToString());
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