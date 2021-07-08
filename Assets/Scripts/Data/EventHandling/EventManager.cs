using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public class EventManager : MonoBehaviour
	{
		private static EventManager instance;
		public static EventManager Instance => instance;

        private Dictionary<Type, List<SpecialAction>> handlers = new Dictionary<Type, List<SpecialAction>>();

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        public void Clear()
		{
            handlers.Clear();
		}

        public void Dispatch(ISimulationEvent simEvent)
		{
            List<SpecialAction> handlerList;

            if(handlers.TryGetValue(simEvent.GetType(), out handlerList))
			{
                for(int index = 0, maxIndex = handlerList.Count; index < maxIndex; index++)
				{
                    if(handlerList[index] != null)
					{
                        handlerList[index].Invoke(simEvent);
					}
				}
			}
		}

        public void AddEventHandler(Type eventType, Action handler)
		{
            var specialAction = new ParameterlessSpecialAction();
            specialAction.Handler = handler;

            RegisterAction(eventType, specialAction);
		}

        public void AddEventHandler(Type eventType, Action<ISimulationEvent> handler)
		{
            var specialAction = new SpecificSpecialAction<ISimulationEvent>();
            specialAction.Handler = handler;

            RegisterAction(eventType, specialAction);
		}

        public void AddEventHandler<T>(Action handler) where T : ISimulationEvent
		{
            AddEventHandler(typeof(T), handler);
		}

        public void AddEventHandler<T>(Action<T> handler) where T : ISimulationEvent
		{
            var specialAction = new SpecificSpecialAction<T>();
            specialAction.Handler = handler;

            RegisterAction(typeof(T), specialAction);
        }

        public List<SpecialAction> RemoveEventHandler(Type eventType, object handler)
		{
            List<SpecialAction> handlerList;

            if(handlers.TryGetValue(eventType, out handlerList))
			{
                var index = handlerList.FindIndex(i => (i != null) && i.Target.Equals(handler));
                if(index >= 0)
				{
                    handlerList[index] = null;
				}
                else
				{
                    handlerList = null;
				}
			}

            return handlerList;
		}

        private void RegisterAction(Type eventType, SpecialAction action)
		{
            if(!handlers.ContainsKey(eventType))
			{
                handlers.Add(eventType, new List<SpecialAction>());
			}

            var handlerList = handlers[eventType];

            handlerList.Add(action);
		}
    }
}