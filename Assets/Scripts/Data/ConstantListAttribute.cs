using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utilities
{
	[AttributeUsage(AttributeTargets.Field)]
	public class ConstantListAttribute : PropertyAttribute
	{
		public IEnumerable<Type> Types { get; private set; }
		public bool Inherited { get; set; }
		public ConstantListAttribute(params Type[] types)
		{
			this.Types = types;
		}
	}
}
