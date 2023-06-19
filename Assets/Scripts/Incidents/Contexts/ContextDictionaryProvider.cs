using Game.Incidents;
using System;
using System.Collections.Generic;

namespace Game.Simulation
{
	public static class ContextDictionaryProvider
	{
		public static IncidentContextDictionary CurrentContexts => GetCurrentContexts.Invoke();
		public static IncidentContextDictionary AllContexts => GetAllContexts.Invoke();
		public static Dictionary<string, ExpressionValue> CurrentExpressionValues { get; set; }
		public static int NextID => nextID;

		private static Func<IncidentContextDictionary> GetCurrentContexts;
		private static Func<IncidentContextDictionary> GetAllContexts;

		private static IncidentContextDictionary contextsToAdd = new IncidentContextDictionary();
		private static IncidentContextDictionary contextsToRemove = new IncidentContextDictionary();

		private static int nextID = 1;

		private static void SetCurrentContextsProvider(Func<IncidentContextDictionary> func)
		{
			GetCurrentContexts = func;
		}

		private static void SetAllContextsProvider(Func<IncidentContextDictionary> func)
		{
			GetAllContexts = func;
		}

		public static void SetContextsProviders(Func<IncidentContextDictionary> funcCurrent, Func<IncidentContextDictionary> funcAll)
		{
			SetCurrentContextsProvider(funcCurrent);
			SetAllContextsProvider(funcAll);

			EventManager.Instance.RemoveEventHandler<AddContextEvent>(OnAddContextEvent);
			EventManager.Instance.RemoveEventHandler<RemoveContextEvent>(OnRemoveContextEvent);

			EventManager.Instance.AddEventHandler<AddContextEvent>(OnAddContextEvent);
			EventManager.Instance.AddEventHandler<RemoveContextEvent>(OnRemoveContextEvent);
		}

		public static void SetNextID(int id)
		{
			nextID = id;
		}

		public static void AddContext<T>(T context) where T : IIncidentContext
		{
			context.ID = GetNextID();
			contextsToAdd[typeof(T)].Add(context);
		}

		public static void AddContextImmediate<T>(T context) where T : IIncidentContext
		{
			AddContext(context);
			DelayedAddContexts();
		}

		public static void RemoveContext<T>(T context) where T : IIncidentContext
		{
			contextsToRemove[typeof(T)].Add(context);
		}

		private static void OnAddContextEvent(AddContextEvent gameEvent)
		{
			AddContext(gameEvent.context);
		}

		private static void OnRemoveContextEvent(RemoveContextEvent gameEvent)
		{
			RemoveContext(gameEvent.context);
		}

		public static void DelayedAddContexts()
		{
			foreach (var contextList in contextsToAdd.Values)
			{
				foreach (var context in contextList)
				{
					CurrentContexts[context.ContextType].Add(context);
					AllContexts[context.ContextType].Add(context);
				}
				contextList.Clear();
			}
		}

		public static void DelayedRemoveContexts()
		{
			foreach (var contextList in contextsToRemove.Values)
			{
				foreach (var context in contextList)
				{
					CurrentContexts[context.ContextType].Remove(context);
				}
				contextList.Clear();
			}
		}

		private static int GetNextID()
		{
			var next = nextID;
			nextID++;
			return next;
		}
	}
}
