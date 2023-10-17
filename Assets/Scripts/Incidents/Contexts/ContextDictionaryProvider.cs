using Game.Data;
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
			EventManager.Instance.RemoveEventHandler<AddContextImmediateEvent>(OnAddContextImmediateEvent);
			EventManager.Instance.RemoveEventHandler<RemoveContextEvent>(OnRemoveContextEvent);

			EventManager.Instance.AddEventHandler<AddContextEvent>(OnAddContextEvent);
			EventManager.Instance.AddEventHandler<AddContextImmediateEvent>(OnAddContextImmediateEvent);
			EventManager.Instance.AddEventHandler<RemoveContextEvent>(OnRemoveContextEvent);
		}

		public static void SetNextID(int id)
		{
			nextID = id;
		}

		private static void AddContext<T>(T context, Type type) where T : IIncidentContext
		{
			context.ID = GetNextID();
			contextsToAdd[type].Add(context);
		}

		private static void AddContext<T>(T context) where T : IIncidentContext
		{
			AddContext(context, context.GetType());
		}

		private static void AddContextImmediate<T>(T context, Type type) where T : IIncidentContext
		{
			AddContext(context, type);
			DelayedAddContexts();
		}

		private static void AddContextImmediate<T>(T context) where T : IIncidentContext
		{
			AddContextImmediate(context, context.GetType());
		}

		private static void RemoveContext<T>(T context, Type type) where T : IIncidentContext
		{
			contextsToRemove[type].Add(context);
		}

		private static void OnAddContextEvent(AddContextEvent gameEvent)
		{
			AddContext(gameEvent.context, gameEvent.contextType);
		}

		private static void OnAddContextImmediateEvent(AddContextImmediateEvent gameEvent)
		{
			AddContextImmediate(gameEvent.context, gameEvent.contextType);
		}

		private static void OnRemoveContextEvent(RemoveContextEvent gameEvent)
		{
			RemoveContext(gameEvent.context, gameEvent.contextType);
		}

		public static void DelayedAddContexts()
		{
			foreach (var pair in contextsToAdd)
			{
				foreach (var context in pair.Value)
				{
					CurrentContexts[pair.Key].Add(context);
					AllContexts[pair.Key].Add(context);
				}
				pair.Value.Clear();
			}
		}

		public static void DelayedRemoveContexts()
		{
			foreach (var pair in contextsToRemove)
			{
				foreach (var context in pair.Value)
				{
					CurrentContexts[pair.Key].Remove(context);
				}
				pair.Value.Clear();
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
