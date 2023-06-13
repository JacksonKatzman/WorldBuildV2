using System;
using System.Linq;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("Items")]
	public class ES3UserType_Inventory : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Inventory() : base(typeof(Game.Incidents.Inventory)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Game.Incidents.Inventory)obj;
			
			writer.WriteProperty("Items", instance.Items.Select(x => x.ID).ToList(), ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Game.Incidents.Inventory)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "Items":
						instance.AddContextIdBuffer("Items", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new Game.Incidents.Inventory();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_InventoryArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_InventoryArray() : base(typeof(Game.Incidents.Inventory[]), ES3UserType_Inventory.Instance)
		{
			Instance = this;
		}
	}
}