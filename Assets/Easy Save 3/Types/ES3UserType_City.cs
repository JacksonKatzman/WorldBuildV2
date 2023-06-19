using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("CurrentLocation", "AffiliatedFaction", "Population", "Wealth", "Resources", "Landmarks", "Characters", "Inventory", "NumIncidents", "Name", "ID")]
	public class ES3UserType_City : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_City() : base(typeof(Game.Incidents.City)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Game.Incidents.City)obj;
			
			writer.WriteProperty("CurrentLocation", new List<int>() { instance.CurrentLocation.ID }, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("AffiliatedFaction", new List<int>() { instance.AffiliatedFaction.ID }, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("Population", instance.Population, ES3Type_int.Instance);
			writer.WriteProperty("Wealth", instance.Wealth, ES3Type_int.Instance);
			writer.WriteProperty("Resources", instance.Resources.Select(x => x.ID).ToList(), ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("Landmarks", instance.Landmarks.Select(x => x.ID).ToList(), ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("Characters", instance.Characters.Select(x => x.ID).ToList(), ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("Inventory", instance.CurrentInventory, ES3UserType_Inventory.Instance);
			writer.WriteProperty("NumIncidents", instance.NumIncidents, ES3Type_int.Instance);
			writer.WriteProperty("Name", instance.Name, ES3Type_string.Instance);
			writer.WriteProperty("ID", instance.ID, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Game.Incidents.City)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "CurrentLocation":
						instance.AddContextIdBuffer("CurrentLocation", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					case "AffiliatedFaction":
						instance.AddContextIdBuffer("AffiliatedFaction", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					case "Population":
						instance.Population = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Wealth":
						instance.Wealth = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Resources":
						instance.AddContextIdBuffer("Resources", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					case "Landmarks":
						instance.AddContextIdBuffer("Landmarks", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					case "Characters":
						instance.AddContextIdBuffer("Characters", reader.Read<System.Collections.Generic.List<System.Int32>>());
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
			var instance = new Game.Incidents.City();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_CityArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CityArray() : base(typeof(Game.Incidents.City[]), ES3UserType_City.Instance)
		{
			Instance = this;
		}
	}
}