using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Game.Generators.Items
{
	public class Relic : Item
	{
		//List of magical auras/properties etc - actions it takes when time advanced?
		public List<MethodInfo> activeEffects;
		public List<MethodInfo> persistentEffects;

		public Relic(string name, Material material, List<MethodInfo> activeEffects, List<MethodInfo>persistentEffects)
		{
			this.name = name;
			this.material = material;
			this.activeEffects = activeEffects;
			this.persistentEffects = persistentEffects;
		}

		public void Activate()
		{
			foreach(var method in activeEffects)
			{
				method.Invoke(null, new object[] { });
			}
		}
	}
}