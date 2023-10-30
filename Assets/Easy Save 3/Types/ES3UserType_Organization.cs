using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("racesAllowedToHoldOffice", "primaryOrganization", "template", "AffiliatedFaction", "NumIncidents", "Name", "ID")]
	public class ES3UserType_Organization : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Organization() : base(typeof(Game.Incidents.Organization)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Game.Incidents.Organization)obj;
			
			writer.WriteProperty("racesAllowedToHoldOffice", instance.racesAllowedToHoldOffice, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<Game.Incidents.Race>)));
			writer.WriteProperty("primaryOrganization", instance.primaryOrganization, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(Game.Incidents.SubOrganization)));
			writer.WritePropertyByRef("template", instance.template);
			writer.WriteProperty("AffiliatedFaction", instance.AffiliatedFaction, ES3UserType_Faction.Instance);
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
					
					case "racesAllowedToHoldOffice":
						instance.racesAllowedToHoldOffice = reader.Read<System.Collections.Generic.List<Game.Incidents.Race>>();
						break;
					case "primaryOrganization":
						instance.primaryOrganization = reader.Read<Game.Incidents.SubOrganization>();
						break;
					case "template":
						instance.template = reader.Read<Game.Incidents.OrganizationTemplate>();
						break;
					case "AffiliatedFaction":
						instance.AffiliatedFaction = reader.Read<Game.Incidents.Faction>(ES3UserType_Faction.Instance);
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