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

			if (GUILayout.Button("REFLECT!"))
			{

				Type contextType = typeof(FactionContext);
				Type type = typeof(Foo);
				var allActions = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();
				var onlyGenerics = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract && x.IsGenericType).ToList();
				var otherMethod = GetAllTypesImplementingOpenGenericType(typeof(IIncidentContext), Assembly.GetExecutingAssembly()).ToList();
				//var typesThatMatch = allActions.Where((IncidentAction)x => )

				foreach (var t in otherMethod)
				{
					var isGT = t.BaseType.IsGenericType;
					if (isGT && t.BaseType.GetGenericArguments()[0] == contextType)
					{
						OutputLogger.Log(t.FullName);
					}
				}

			}

			if (GUILayout.Button("Test Faction"))
			{
				var faction = new Faction();
				faction.context.Population = 14;

				var faction2 = new Faction();
				faction2.context.Population = 5;

				var simMan = SimulationManager.Instance;

				simMan.CreateDebugWorld();
				simMan.Providers[typeof(FactionContext)].Add(faction);
				simMan.Providers[typeof(FactionContext)].Add(faction2);

				faction.DeployContext();
			}

			if(GUILayout.Button("In game test!"))
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


	public interface Foo
	{

	}

	abstract public class Bar<T> : Foo where T : IIncidentContext
	{
		//abstract public Type ContextType { get; }
		public Type ContextType => typeof(T);
	}

	public class Goo : Bar<FactionContext>
	{
		//override public Type ContextType => typeof(FactionContext);
	}

	public class Ga : Bar<FactionContext>
	{
		//override public Type ContextType => typeof(FactionContext);
	}

}