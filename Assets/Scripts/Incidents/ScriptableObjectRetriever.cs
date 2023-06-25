﻿using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Incidents
{
	public class ScriptableObjectRetriever<T> where T: SerializedScriptableObject
	{
		//need a way to find the container object for the dictionary of scriptable objects of whatever type T is
		//then we have a string field that has a dropdown built of all the dictionary keys or w/e
		//could use an interface to ensure we grab by name of scriptable object instead idk

		[ValueDropdown("GetKeys"), OnValueChanged("OnValueChanged")]
		public string prefabKey;

		[HideInInspector]
		public Action onChanged;

		public T RetrieveObject()
		{
			if (SerializedObjectContainerService.Instance.container.objects.TryGetValue(typeof(T), out var dict))
			{
				if (dict.TryGetValue(prefabKey, out var value))
				{
					return (T)value;
				}
				else
				{
					OutputLogger.LogError($"{GetType().ToString()} failed to find key: {prefabKey} for type {typeof(T).ToString()}.");
					return default(T);
				}
			}
			else
			{
				OutputLogger.LogError($"{GetType().ToString()} failed to find key: {typeof(T).ToString()}.");
				return default(T);
			}
		}

		private IEnumerable<string> GetKeys()
		{
			if(SerializedObjectContainerService.Instance.container.objects.TryGetValue(typeof(T), out var dict))
			{
				return dict.Keys;
			}
			else
			{
				return new List<string>();
			}
		}

		private void OnValueChanged()
		{
			onChanged?.Invoke();
		}
	}
}