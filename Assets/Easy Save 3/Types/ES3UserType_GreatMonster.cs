using Game.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("dataBlock", "Inventory", "CharacterName", "LawfulChaoticAlignmentAxis", "GoodEvilAlignmentAxis", "AffiliatedFaction", "ID")]
	public class ES3UserType_GreatMonster : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_GreatMonster() : base(typeof(Game.Incidents.GreatMonster)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Game.Incidents.GreatMonster)obj;
			
			writer.WritePropertyByRef("dataBlock", instance.dataBlock);
			writer.WritePrivateProperty("Inventory", instance);
			writer.WriteProperty("CharacterName", instance.CharacterName, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(Game.Generators.Names.CharacterName)));
			writer.WriteProperty("LawfulChaoticAlignmentAxis", instance.LawfulChaoticAlignmentAxis, ES3Type_int.Instance);
			writer.WriteProperty("GoodEvilAlignmentAxis", instance.GoodEvilAlignmentAxis, ES3Type_int.Instance);
			writer.WriteProperty("AffiliatedFaction", new List<int>() { instance.AffiliatedFaction.ID }, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("ID", instance.ID, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Game.Incidents.GreatMonster)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "dataBlock":
						instance.dataBlock = reader.Read<MonsterData>();
						break;
					case "Inventory":
					instance = (Game.Incidents.GreatMonster)reader.SetPrivateProperty("Inventory", reader.Read<Game.Incidents.Inventory>(), instance);
					break;
					case "CharacterName":
						instance.CharacterName = reader.Read<Game.Generators.Names.CharacterName>();
						break;
					case "LawfulChaoticAlignmentAxis":
						instance.LawfulChaoticAlignmentAxis = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "GoodEvilAlignmentAxis":
						instance.GoodEvilAlignmentAxis = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "AffiliatedFaction":
						instance.AddContextIdBuffer("AffiliatedFaction", reader.Read<System.Collections.Generic.List<System.Int32>>());
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
			var instance = new Game.Incidents.GreatMonster();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_GreatMonsterArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_GreatMonsterArray() : base(typeof(Game.Incidents.GreatMonster[]), ES3UserType_GreatMonster.Instance)
		{
			Instance = this;
		}
	}
}