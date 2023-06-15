using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute()]
	public class ES3UserType_IncidentContextDictionary : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3UserType_IncidentContextDictionary() : base(typeof(Game.Simulation.IncidentContextDictionary)){ Instance = this; priority = 1; }


		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (Game.Simulation.IncidentContextDictionary)obj;
			writer.WriteProperty("Keys", instance.Keys.ToArray());
			writer.WriteProperty("Values", instance.Values.ToArray());
		}

		public override object Read<T>(ES3Reader reader)
		{
			var instance = new Game.Simulation.IncidentContextDictionary();

			var Keys = reader.ReadProperty<Type[]>();
			var Values = reader.ReadProperty<List<Game.Incidents.IIncidentContext>[]>();

			for (int i = 0; i < Keys.Length; i++)
				instance.Add(Keys[i], Values[i]);

			return instance;
		}

		/*
		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Game.Simulation.IncidentContextDictionary)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new Game.Simulation.IncidentContextDictionary();
			ReadObject<T>(reader, instance);
			return instance;
		}
		*/
	}


	public class ES3UserType_IncidentContextDictionaryArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_IncidentContextDictionaryArray() : base(typeof(Game.Simulation.IncidentContextDictionary[]), ES3UserType_IncidentContextDictionary.Instance)
		{
			Instance = this;
		}
	}
}