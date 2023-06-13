using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Game.Simulation;
using Game.Incidents;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("CharacterName", "Age", "Gender", "Race", "AffiliatedFaction", "OfficialPosition", "PoliticalPriority", "EconomicPriority", "ReligiousPriority", "MilitaryPriority", "Influence", "Wealth", "Strength", "Dexterity", "Constitution", "Intelligence", "Wisdom", "Charisma", "Inventory", "Parents", "Spouses", "Siblings", "Children", "Info", "LawfulChaoticAlignmentAxis", "GoodEvilAlignmentAxis", "MajorCharacter", "Possessed", "ID")]
	public class ES3UserType_Character : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Character() : base(typeof(Game.Incidents.Character)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Game.Incidents.Character)obj;
			
			writer.WriteProperty("CharacterName", instance.CharacterName, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(Game.Generators.Names.CharacterName)));
			writer.WriteProperty("Age", instance.Age, ES3Type_int.Instance);
			writer.WriteProperty("Gender", instance.Gender, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(Game.Enums.Gender)));
			writer.WriteProperty("Race", new List<int>() { instance.Race.ID }, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("AffiliatedFaction", new List<int>() { instance.AffiliatedFaction.ID }, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("Organization", new List<int>() { instance.Organization.ID }, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("PoliticalPriority", instance.PoliticalPriority, ES3Type_int.Instance);
			writer.WriteProperty("EconomicPriority", instance.EconomicPriority, ES3Type_int.Instance);
			writer.WriteProperty("ReligiousPriority", instance.ReligiousPriority, ES3Type_int.Instance);
			writer.WriteProperty("MilitaryPriority", instance.MilitaryPriority, ES3Type_int.Instance);
			writer.WriteProperty("Influence", instance.Influence, ES3Type_int.Instance);
			writer.WriteProperty("Wealth", instance.Wealth, ES3Type_int.Instance);
			writer.WriteProperty("Strength", instance.Strength, ES3Type_int.Instance);
			writer.WriteProperty("Dexterity", instance.Dexterity, ES3Type_int.Instance);
			writer.WriteProperty("Constitution", instance.Constitution, ES3Type_int.Instance);
			writer.WriteProperty("Intelligence", instance.Intelligence, ES3Type_int.Instance);
			writer.WriteProperty("Wisdom", instance.Wisdom, ES3Type_int.Instance);
			writer.WriteProperty("Charisma", instance.Charisma, ES3Type_int.Instance);
			writer.WriteProperty("Inventory", instance.Inventory, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(Game.Incidents.Inventory)));

			writer.WriteProperty("Parents", instance.Parents.Select(x => x.ID).ToList(), ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("Spouses", instance.Spouses.Select(x => x.ID).ToList(), ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("Siblings", instance.Siblings.Select(x => x.ID).ToList(), ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("Children", instance.Children.Select(x => x.ID).ToList(), ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));

			writer.WriteProperty("LawfulChaoticAlignmentAxis", instance.LawfulChaoticAlignmentAxis, ES3Type_int.Instance);
			writer.WriteProperty("GoodEvilAlignmentAxis", instance.GoodEvilAlignmentAxis, ES3Type_int.Instance);
			writer.WriteProperty("MajorCharacter", instance.MajorCharacter, ES3Type_bool.Instance);
			writer.WriteProperty("Possessed", instance.Possessed, ES3Type_bool.Instance);
			writer.WriteProperty("ID", instance.ID, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Game.Incidents.Character)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "CharacterName":
						instance.CharacterName = reader.Read<Game.Generators.Names.CharacterName>();
						break;
					case "Age":
						instance.Age = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Gender":
						instance.Gender = reader.Read<Game.Enums.Gender>();
						break;
					case "Race":
						instance.AddContextIdBuffer("Race", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					case "AffiliatedFaction":
						instance.AddContextIdBuffer("AffiliatedFaction", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					case "Organization":
						instance.AddContextIdBuffer("Organization", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
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
					case "Influence":
						instance.Influence = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Wealth":
						instance.Wealth = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Strength":
						instance.Strength = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Dexterity":
						instance.Dexterity = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Constitution":
						instance.Constitution = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Intelligence":
						instance.Intelligence = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Wisdom":
						instance.Wisdom = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Charisma":
						instance.Charisma = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Inventory":
						instance.Inventory = reader.Read<Game.Incidents.Inventory>();
						break;
					case "Parents":
						instance.AddContextIdBuffer("Parents", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					case "Spouses":
						instance.AddContextIdBuffer("Spouses", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					case "Siblings":
						instance.AddContextIdBuffer("Siblings", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					case "Children":
						instance.AddContextIdBuffer("Children", reader.Read<System.Collections.Generic.List<System.Int32>>());
						break;
					case "LawfulChaoticAlignmentAxis":
						instance.LawfulChaoticAlignmentAxis = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "GoodEvilAlignmentAxis":
						instance.GoodEvilAlignmentAxis = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "MajorCharacter":
						instance.MajorCharacter = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "Possessed":
						instance.Possessed = reader.Read<System.Boolean>(ES3Type_bool.Instance);
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
			var instance = new Game.Incidents.Character();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_CharacterArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CharacterArray() : base(typeof(Game.Incidents.Character[]), ES3UserType_Character.Instance)
		{
			Instance = this;
		}
	}
}