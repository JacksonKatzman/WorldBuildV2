using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("racePreset", "MinAge", "MaxAge", "NumIncidents", "Name", "ID")]
	public class ES3UserType_Race : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Race() : base(typeof(Game.Incidents.Race)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Game.Incidents.Race)obj;
			
			writer.WritePropertyByRef("racePreset", instance.racePreset);
			writer.WriteProperty("MinAge", instance.MinAge, ES3Type_int.Instance);
			writer.WriteProperty("MaxAge", instance.MaxAge, ES3Type_int.Instance);
			writer.WriteProperty("NumIncidents", instance.NumIncidents, ES3Type_int.Instance);
			writer.WriteProperty("Name", instance.Name, ES3Type_string.Instance);
			writer.WriteProperty("ID", instance.ID, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Game.Incidents.Race)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "racePreset":
						instance.racePreset = reader.Read<Game.Simulation.RacePreset>();
						break;
					case "MinAge":
						instance.MinAge = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "MaxAge":
						instance.MaxAge = reader.Read<System.Int32>(ES3Type_int.Instance);
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
			var instance = new Game.Incidents.Race();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_RaceArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_RaceArray() : base(typeof(Game.Incidents.Race[]), ES3UserType_Race.Instance)
		{
			Instance = this;
		}
	}
}