using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("ArmorType", "FlatBonus", "ItemGrade", "CurrentInventory", "Value", "Points", "NumIncidents", "Name", "ID")]
	public class ES3UserType_Armor : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Armor() : base(typeof(Game.Generators.Items.Armor)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Game.Generators.Items.Armor)obj;
			
			writer.WritePropertyByRef("ArmorType", instance.ArmorType);
			writer.WriteProperty("FlatBonus", instance.FlatBonus, ES3Type_int.Instance);
			writer.WriteProperty("ItemGrade", instance.ItemGrade, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(Game.Enums.ItemGrade)));
			writer.WriteProperty("CurrentInventory", instance.CurrentInventory, ES3UserType_Inventory.Instance);
			writer.WriteProperty("Value", instance.Value, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(Game.Generators.Items.ItemValue)));
			writer.WriteProperty("Points", instance.Points, ES3Type_int.Instance);
			writer.WriteProperty("NumIncidents", instance.NumIncidents, ES3Type_int.Instance);
			writer.WriteProperty("Name", instance.Name, ES3Type_string.Instance);
			writer.WriteProperty("ID", instance.ID, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Game.Generators.Items.Armor)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "ArmorType":
						instance.ArmorType = reader.Read<Game.Generators.Items.ArmorType>();
						break;
					case "FlatBonus":
						instance.FlatBonus = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "ItemGrade":
						instance.ItemGrade = reader.Read<Game.Enums.ItemGrade>();
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
			var instance = new Game.Generators.Items.Armor();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_ArmorArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ArmorArray() : base(typeof(Game.Generators.Items.Armor[]), ES3UserType_Armor.Instance)
		{
			Instance = this;
		}
	}
}