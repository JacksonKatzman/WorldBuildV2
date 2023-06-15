using System;
using System.Collections.Generic;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("IncidentID", "ParentID", "Contexts", "ReportYear", "ReportLog")]
	public class ES3UserType_IncidentReport : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_IncidentReport() : base(typeof(Game.Incidents.IncidentReport)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Game.Incidents.IncidentReport)obj;
			
			writer.WriteProperty("IncidentID", instance.IncidentID, ES3Type_int.Instance);
			writer.WriteProperty("ParentID", instance.ParentID, ES3Type_int.Instance);
			var idDictionary = new Dictionary<string, int>();
			foreach(var pair in instance.Contexts)
			{
				if (pair.Value != null)
				{
					idDictionary.Add(pair.Key, pair.Value.ID);
				}
			}
			writer.WriteProperty("Contexts", idDictionary, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.Dictionary<System.String, System.Int32>)));
			writer.WriteProperty("ReportYear", instance.ReportYear, ES3Type_int.Instance);
			writer.WriteProperty("ReportLog", instance.ReportLog, ES3Type_string.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Game.Incidents.IncidentReport)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "IncidentID":
						instance.IncidentID = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "ParentID":
						instance.ParentID = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Contexts":
						instance.LoadContextProperties(reader.Read<System.Collections.Generic.Dictionary<System.String, System.Int32>>());
						break;
					case "ReportYear":
						instance.ReportYear = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "ReportLog":
						instance.ReportLog = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new Game.Incidents.IncidentReport();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_IncidentReportArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_IncidentReportArray() : base(typeof(Game.Incidents.IncidentReport[]), ES3UserType_IncidentReport.Instance)
		{
			Instance = this;
		}
	}
}