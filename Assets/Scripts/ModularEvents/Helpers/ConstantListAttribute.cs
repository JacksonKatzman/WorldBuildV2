using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ModularEvents
{
	[AttributeUsage(AttributeTargets.Field)]
	public class ConstantListAttribute : UnityEngine.PropertyAttribute
	{
		public IEnumerable<Type> Types { get; private set; }
		public bool Inherited { get; set; }

		public ConstantListAttribute(params Type[] types)
		{
			Types = types;
		}
	}
}