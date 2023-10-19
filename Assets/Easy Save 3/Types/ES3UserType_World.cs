using Game.Data;
using Game.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("nextID", "CurrentContexts", "AllContexts", "ID", "Age")]
	public class ES3UserType_World : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_World() : base(typeof(Game.Simulation.World)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Game.Simulation.World)obj;
			
			writer.WriteProperty("nextID", ContextDictionaryProvider.NextID, ES3Type_int.Instance);
			//writer.WritePrivateProperty("CurrentContexts", instance);
			writer.WritePrivateProperty("AllContexts", instance);
			writer.WriteProperty("ID", instance.ID, ES3Type_int.Instance);
			writer.WriteProperty("Age", instance.Age, ES3Type_int.Instance);

			var currentContexts = new List<int>();
			foreach(var typeListPair in instance.CurrentContexts)
			{
				currentContexts.AddRange(typeListPair.Value.Select(x => x.ID));
			}

			writer.WriteProperty("CurrentContexts", currentContexts, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("LostItems", instance.LostItems.Select(x => x.ID).ToList(), ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Game.Simulation.World)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "nextID":
						ContextDictionaryProvider.SetNextID(reader.Read<System.Int32>(ES3Type_int.Instance));
						break;
					case "CurrentContexts":
						instance.AddContextIdBuffer("CurrentContexts", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					case "AllContexts":
						instance = (Game.Simulation.World)reader.SetPrivateProperty("AllContexts", reader.Read<IncidentContextDictionary>(), instance);
						break;
					case "ID":
						instance.ID = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Age":
						instance.Age = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "LostItems":
						instance.AddContextIdBuffer("LostItems", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new Game.Simulation.World();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_WorldArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_WorldArray() : base(typeof(Game.Simulation.World[]), ES3UserType_World.Instance)
		{
			Instance = this;
		}
	}
}