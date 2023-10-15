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

		virtual public Type ContextType => this.GetType();

		virtual public int NumIncidents { get; set; }
		virtual public string Name { get; set; }

		virtual public int ID { get; set; }

		virtual public int ParentID => -1;

		protected Dictionary<string, List<int>> contextIDLoadBuffers;

		public IncidentContext()
		{
			SetupHistoricalData();
		}

		virtual public void UpdateHistoricalData()
		{
			var year = SimulationManager.Instance.world.Age;

			foreach (var property in propertyList)
			{
				var value = (int)property.GetValue(this);
				historicalData[property.Name].Add(new YearData<int>(year, value));
			}
		}

		abstract public void UpdateContext();

		abstract public void DeployContext();

		abstract public void Die();

		virtual public DataTable GetDataTable()
		{
			var table1 = new DataTable();

			if(historicalData.Count == 0)
			{
				return table1;
			}

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

			foreach (var property in propertyList)
			{
				historicalData.Add(property.Name, new List<YearData<int>>());
			}
		}

		private List<PropertyInfo> GetPropertyList()
		{
			var propertyInfo = GetType().GetProperties();
			var interfacePropertyInfo = typeof(IIncidentContext).GetProperties();

			return propertyInfo.Where(x => !interfacePropertyInfo.Any(y => x.Name == y.Name)).ToList();
		}

		private List<PropertyInfo> GetIntegerPropertyList()
		{
			return GetPropertyList().Where(x => x.PropertyType == typeof(int)).ToList();
		}

		public void AddContextIdBuffer(string key, List<int> ids)
		{
			if(contextIDLoadBuffers == null)
			{
				contextIDLoadBuffers = new Dictionary<string, List<int>>();
			}

			contextIDLoadBuffers.Add(key, ids);
		}

		public virtual void LoadContextProperties() { }
	}
}