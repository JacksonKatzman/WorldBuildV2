using System;
using System.Collections.Generic;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("CurrentLocation", "Inventory", "NumIncidents", "Name", "ID")]
	public class ES3UserType_Landmark : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Landmark() : base(typeof(Game.Incidents.Landmark)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Game.Incidents.Landmark)obj;

			writer.WriteProperty("CurrentLocation", new List<int>() { instance.CurrentLocation.ID }, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("Inventory", instance.CurrentInventory, ES3UserType_Inventory.Instance);
			writer.WriteProperty("NumIncidents", instance.NumIncidents, ES3Type_int.Instance);
			writer.WriteProperty("Name", instance.Name, ES3Type_string.Instance);
			writer.WriteProperty("ID", instance.ID, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Game.Incidents.Landmark)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "CurrentLocation":
						instance.AddContextIdBuffer("CurrentLocation", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					case "Inventory":
						instance.CurrentInventory = reader.Read<Game.Incidents.Inventory>(ES3UserType_Inventory.Instance);
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
			var instance = new Game.Incidents.Landmark();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_LandmarkArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_LandmarkArray() : base(typeof(Game.Incidents.Landmark[]), ES3UserType_Landmark.Instance)
		{
			Instance = this;
		}
	}
}