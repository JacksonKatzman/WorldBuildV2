﻿
using Game.Incidents;
using System;
using System.Collections.Generic;

namespace Game.Simulation
{
	public class EventManager
    {
        private static EventManager instance;
        public static EventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EventManager();
                }
                return instance;
            }
        }

        private Dictionary<Type, List<ISimulationEventHandler>> handlers = new Dictionary<Type, List<ISimulationEventHandler>>();

        public void Clear()
        {
            handlers.Clear();
        }

        public void Dispatch(ISimulationEvent simEvent)
        {
            List<ISimulationEventHandler> handlerList;

            if (handlers.TryGetValue(simEvent.GetType(), out handlerList))
            {
                for (int index = 0; index < handlerList.Count; index++)
                {
                    if (handlerList[index] != null)
                    {
                        handlerList[index].Invoke(simEvent);
                    }
                }
            }
        }

        public void AddEventHandler(Type eventType, Action handler)
        {
            var specialAction = new SimulationEventHandler();
            specialAction.Handler = handler;

            RegisterAction(eventType, specialAction);
        }

        public void AddEventHandler(Type eventType, Action<ISimulationEvent> handler)
        {
            var specialAction = new SimulationEventHandler<ISimulationEvent>();
            specialAction.Handler = handler;

            RegisterAction(eventType, specialAction);
        }

        public void AddEventHandler<T>(Action handler) where T : ISimulationEvent
        {
            AddEventHandler(typeof(T), handler);
        }

        public void AddEventHandler<T>(Action<T> handler) where T : ISimulationEvent
        {
            var specialAction = new SimulationEventHandler<T>();
            specialAction.Handler = handler;

            RegisterAction(typeof(T), specialAction);
        }

        public List<ISimulationEventHandler> RemoveEventHandler(Type eventType, object handler)
        {
            List<ISimulationEventHandler> handlerList;

            if (handlers.TryGetValue(eventType, out handlerList))
            {
                var index = handlerList.FindIndex(i => (i != null) && i.Target.Equals(handler));
                if (index >= 0)
                {
                    handlerList.RemoveAt(index);
                }
                else
                {
                    handlerList = null;
                }
            }

            return handlerList;
        }

		public void RemoveEventHandler<T>(Action<T> handler)
		{
            RemoveEventHandler(typeof(T), handler);
        }

		public List<ISimulationEventHandler> RemoveEventHandler<T>(object handler) where T : ISimulationEvent
		{
            return RemoveEventHandler(typeof(T), handler);
		}

        private void RegisterAction(Type eventType, ISimulationEventHandler action)
        {
            if (!handlers.ContainsKey(eventType))
            {
                handlers.Add(eventType, new List<ISimulationEventHandler>());
            }

            var handlerList = handlers[eventType];

            handlerList.Add(action);
        }
    }

    public interface ISimulationEventHandler
	{
		object Target { get; }
        void Invoke(ISimulationEvent simulationEvent);
	}

    public class SimulationEventHandler : ISimulationEventHandler
	{
        public object Target { get { return Handler; } }
        public Action Handler { get; set; }

        public void Invoke(ISimulationEvent simEvent)
        {
            Handler.Invoke();
        }
    }

    public class SimulationEventHandler<T> : ISimulationEventHandler where T : ISimulationEvent
    {
        public object Target { get { return Handler; } }
        public Action<T> Handler { get; set; }

        public void Invoke(ISimulationEvent simEvent)
        {
            Handler.Invoke((T)simEvent);
        }
    }

    public interface ISimulationEvent
	{

	}

    public class AddContextEvent : ISimulationEvent
	{
        public IIncidentContext context;
        public Type contextType;
        public bool immediate;

        public AddContextEvent(IIncidentContext context, Type type = null, bool immediate = false)
        {
            this.context = context;
            this.contextType = type == null ? context.GetType() : type;
            this.immediate = immediate;
        }

        public AddContextEvent(IIncidentContext context, bool immediate = false)
        {
            this.context = context;
            this.contextType = context.GetType();
            this.immediate = immediate;
        }
    }

    public class RemoveContextEvent : ISimulationEvent
	{
        public IIncidentContext context;
        public Type contextType;

		public RemoveContextEvent(IIncidentContext context, Type type = null)
		{
			this.context = context;
            this.contextType = type == null ? context.GetType() : type;
        }
	}

    public class AffiliatedFactionChangedEvent : ISimulationEvent
	{
        public IFactionAffiliated affiliate;
        public IFactionAffiliated newFaction;

		public AffiliatedFactionChangedEvent(IFactionAffiliated affiliate, IFactionAffiliated newFaction)
		{
			this.affiliate = affiliate;
			this.newFaction = newFaction;
		}
	}

    public class WarDeclaredEvent : ISimulationEvent
	{
        public IFactionAffiliated attacker;
        public IFactionAffiliated defender;

		public WarDeclaredEvent(IFactionAffiliated attacker, IFactionAffiliated defender)
		{
			this.attacker = attacker;
			this.defender = defender;
		}
	}

    public class PeaceDeclaredEvent : ISimulationEvent
	{
        public IFactionAffiliated declarer;
        public IFactionAffiliated accepter;

		public PeaceDeclaredEvent(IFactionAffiliated declarer, IFactionAffiliated accepter)
		{
			this.declarer = declarer;
			this.accepter = accepter;
		}
	}
}
