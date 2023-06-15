using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("TileIndex", "NumIncidents", "Name", "ID")]
	public class ES3UserType_Location : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Location() : base(typeof(Game.Incidents.Location)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Game.Incidents.Location)obj;
			
			writer.WriteProperty("TileIndex", instance.TileIndex, ES3Type_int.Instance);
			writer.WriteProperty("NumIncidents", instance.NumIncidents, ES3Type_int.Instance);
			writer.WriteProperty("Name", instance.Name, ES3Type_string.Instance);
			writer.WriteProperty("ID", instance.ID, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Game.Incidents.Location)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "TileIndex":
						instance.TileIndex = reader.Read<System.Int32>(ES3Type_int.Instance);
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
			var instance = new Game.Incidents.Location();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_LocationArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_LocationArray() : base(typeof(Game.Incidents.Location[]), ES3UserType_Location.Instance)
		{
			Instance = this;
		}
	}
}