using Game.Simulation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Game.Incidents
{
	public abstract class IncidentContext : IIncidentContext
	{
		protected Dictionary<string, List<YearData<int>>> historicalData;
		protected List<PropertyInfo> propertyList;

		public Type ContextType => this.GetType();

		public int NumIncidents { get; set; }

		virtual public int ID { get; set; }

		public int ParentID => -1;

		public IncidentContext()
		{
			SetupHistoricalData();
		}

		public void UpdateHistoricalData()
		{
			var year = SimulationManager.Instance.world.Age;
			//historicalData["Influence"].Add(new YearData<int>(SimulationManager.Instance.world.Age, Influence));
			foreach (var property in propertyList)
			{
				var value = (int)property.GetValue(this);
				historicalData[property.Name].Add(new YearData<int>(year, value));
			}
		}

		virtual public void UpdateContext()
		{

		}

		virtual public void DeployContext()
		{

		}
		public DataTable GetDataTable()
		{
			var table1 = new DataTable();
			table1.Columns.Add(new DataColumn("ContextID"));
			table1.Columns.Add(new DataColumn("ContextType"));
			table1.Columns.Add(new DataColumn("Property"));
			foreach (var yearData in historicalData.First().Value)
			{
				table1.Columns.Add(new DataColumn(yearData.year.ToString()));
			}

			foreach (var pair in historicalData)
			{
				var dataSet = new List<string>();
				dataSet.Add(ID.ToString());
				dataSet.Add(ContextType.ToString());
				dataSet.Add(pair.Key.ToString());
				foreach (var data in pair.Value)
				{
					dataSet.Add(data.data.ToString());
				}

				table1.Rows.Add(dataSet.ToArray());
			}

			return table1;
		}

		private void SetupHistoricalData()
		{
			propertyList = GetIntegerPropertyList();
			historicalData = new Dictionary<string, List<YearData<int>>>();
			//historicalData.Add("Influence", new List<IYearData>{ new YearData<int>(SimulationManager.Instance.world.Age, Influence) });
			foreach (var property in propertyList)
			{
				historicalData.Add(property.Name, new List<YearData<int>>());
			}
		}

		private List<PropertyInfo> GetPropertyList()
		{
			var propertyInfo = ContextType.GetProperties();
			var interfacePropertyInfo = typeof(IIncidentContext).GetProperties();

			return propertyInfo.Where(x => !interfacePropertyInfo.Any(y => x.Name == y.Name)).ToList();
		}

		private List<PropertyInfo> GetIntegerPropertyList()
		{
			return GetPropertyList().Where(x => x.PropertyType == typeof(int)).ToList();
		}
	}
}