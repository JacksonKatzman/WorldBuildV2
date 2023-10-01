using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("ValueMultiplier", "CurrentInventory", "Value", "Points", "NumIncidents", "Name", "ID")]
	public class ES3UserType_Trinket : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Trinket() : base(typeof(Game.Generators.Items.Trinket)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Game.Generators.Items.Trinket)obj;
			
			writer.WriteProperty("ValueMultiplier", instance.ValueMultiplier, ES3Type_float.Instance);
			writer.WriteProperty("CurrentInventory", instance.CurrentInventory, ES3UserType_Inventory.Instance);
			writer.WriteProperty("Value", instance.Value, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(Game.Generators.Items.ItemValue)));
			writer.WriteProperty("Points", instance.Points, ES3Type_int.Instance);
			writer.WriteProperty("NumIncidents", instance.NumIncidents, ES3Type_int.Instance);
			writer.WriteProperty("Name", instance.Name, ES3Type_string.Instance);
			writer.WriteProperty("ID", instance.ID, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Game.Generators.Items.Trinket)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "ValueMultiplier":
						instance.ValueMultiplier = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "CurrentInventory":
						instance.CurrentInventory = reader.Read<Game.Incidents.Inventory>(ES3UserType_Inventory.Instance);
						break;
					case "Value":
						instance.Value = reader.Read<Game.Generators.Items.ItemValue>();
						break;
					case "Points":
						instance.Points = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "NumIncidents":
						instance.NumIncidents = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Name":
						instance.Name = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "ID":
						instance.ID = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new Game.Generators.Items.Trinket();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_TrinketArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_TrinketArray() : base(typeof(Game.Generators.Items.Trinket[]), ES3UserType_Trinket.Instance)
		{
			Instance = this;
		}
	}
}