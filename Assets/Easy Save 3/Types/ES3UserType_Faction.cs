using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("Population", "Influence", "Wealth", "MilitaryPower", "FactionRelations", "Cities", "PoliticalPriority", "EconomicPriority", "ReligiousPriority", "MilitaryPriority", "LawfulChaoticAlignmentAxis", "GoodEvilAlignmentAxis", "FactionsAtWarWith", "Government", "ControlledTileIndices", "NumIncidents", "Name", "ID")]
	public class ES3UserType_Faction : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Faction() : base(typeof(Game.Incidents.Faction)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Game.Incidents.Faction)obj;
			
			writer.WriteProperty("Influence", instance.Influence, ES3Type_int.Instance);
			writer.WriteProperty("Wealth", instance.Wealth, ES3Type_int.Instance);
			writer.WriteProperty("MilitaryPower", instance.MilitaryPower, ES3Type_int.Instance);
			writer.WriteProperty("FactionRelationKeys", instance.FactionRelations.Keys.Select(x => x.ID).ToList(), ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("FactionRelationValues", instance.FactionRelations.Values.ToList(), ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("Cities", instance.Cities.Select(x => x.ID).ToList(), ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("Priorities", instance.Priorities, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.Dictionary<Game.Enums.OrganizationType, System.Int32>)));
			/*
			writer.WriteProperty("PoliticalPriority", instance.PoliticalPriority, ES3Type_int.Instance);
			writer.WriteProperty("EconomicPriority", instance.EconomicPriority, ES3Type_int.Instance);
			writer.WriteProperty("ReligiousPriority", instance.ReligiousPriority, ES3Type_int.Instance);
			writer.WriteProperty("MilitaryPriority", instance.MilitaryPriority, ES3Type_int.Instance);
			*/
			writer.WriteProperty("LawfulChaoticAlignmentAxis", instance.LawfulChaoticAlignmentAxis, ES3Type_int.Instance);
			writer.WriteProperty("GoodEvilAlignmentAxis", instance.GoodEvilAlignmentAxis, ES3Type_int.Instance);
			writer.WriteProperty("FactionsAtWarWith", instance.FactionsAtWarWith.Select(x => x.ID).ToList(), ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("Government", new List<int>() { instance.Government.ID }, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("ControlledTileIndices", instance.ControlledTileIndices, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("NumIncidents", instance.NumIncidents, ES3Type_int.Instance);
			writer.WriteProperty("Name", instance.Name, ES3Type_string.Instance);
			writer.WriteProperty("ID", instance.ID, ES3Type_int.Instance);
			writer.WriteProperty("NamingTheme", instance.namingTheme, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(Game.Generators.Names.NamingTheme)));
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Game.Incidents.Faction)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "Influence":
						instance.Influence = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Wealth":
						instance.Wealth = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "MilitaryPower":
						instance.MilitaryPower = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "FactionRelationKeys":
						instance.AddContextIdBuffer("FactionRelationKeys", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					case "FactionRelationValues":
						instance.AddContextIdBuffer("FactionRelationValues", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					case "Cities":
						instance.AddContextIdBuffer("Cities", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					case "Priorities":
						instance.Priorities = reader.Read<System.Collections.Generic.Dictionary<Game.Enums.OrganizationType, System.Int32>>();
						break;
						/*
					case "PoliticalPriority":
						instance.PoliticalPriority = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "EconomicPriority":
						instance.EconomicPriority = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "ReligiousPriority":
						instance.ReligiousPriority = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "MilitaryPriority":
						instance.MilitaryPriority = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
						*/
					case "LawfulChaoticAlignmentAxis":
						instance.LawfulChaoticAlignmentAxis = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "GoodEvilAlignmentAxis":
						instance.GoodEvilAlignmentAxis = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "FactionsAtWarWith":
						instance.AddContextIdBuffer("FactionsAtWarWith", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					case "Government":
						instance.AddContextIdBuffer("Government", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					case "ControlledTileIndices":
						instance.ControlledTileIndices = reader.Read<System.Collections.Generic.List<System.Int32>>();
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
					case "NamingTheme":
						instance.namingTheme = reader.Read<Game.Generators.Names.NamingTheme>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new Game.Incidents.Faction();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_FactionArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_FactionArray() : base(typeof(Game.Incidents.Faction[]), ES3UserType_Faction.Instance)
		{
			Instance = this;
		}
	}
}