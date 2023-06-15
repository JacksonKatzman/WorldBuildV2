using System;
using System.Collections.Generic;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("hierarchy", "organizationType", "AffiliatedFaction", "NumIncidents", "Name", "ID")]
	public class ES3UserType_Organization : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Organization() : base(typeof(Game.Incidents.Organization)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Game.Incidents.Organization)obj;
			
			writer.WriteProperty("hierarchy", instance.hierarchy, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<Game.Incidents.OrganizationTier>)));
			writer.WriteProperty("organizationType", instance.organizationType, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(Game.Enums.OrganizationType)));
			writer.WriteProperty("AffiliatedFaction", new List<int>() { instance.AffiliatedFaction.ID }, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("NumIncidents", instance.NumIncidents, ES3Type_int.Instance);
			writer.WriteProperty("Name", instance.Name, ES3Type_string.Instance);
			writer.WriteProperty("ID", instance.ID, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Game.Incidents.Organization)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "hierarchy":
						instance.hierarchy = reader.Read<System.Collections.Generic.List<Game.Incidents.OrganizationTier>>();
						break;
					case "organizationType":
						instance.organizationType = reader.Read<Game.Enums.OrganizationType>();
						break;
					case "AffiliatedFaction":
						instance.AddContextIdBuffer("AffiliatedFaction", reader.Read<System.Collections.Generic.List<System.Int32>>());
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
			var instance = new Game.Incidents.Organization();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_OrganizationArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_OrganizationArray() : base(typeof(Game.Incidents.Organization[]), ES3UserType_Organization.Instance)
		{
			Instance = this;
		}
	}
}