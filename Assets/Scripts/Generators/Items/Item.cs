using Game.Data.EventHandling.EventRecording;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Generators.Items
{
	public abstract class Item : IRecordable
	{
		public string Name => name;

		protected string name;
		protected Material material;
	}
}