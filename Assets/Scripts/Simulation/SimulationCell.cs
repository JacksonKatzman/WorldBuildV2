using Game.Terrain;
using Game.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Game.Simulation
{
	public class SimulationCell
	{
		[NonSerialized]
		public HexCell hexCell;

		public int Index { get; private set; }

		public SimulationCell() { }

		public SimulationCell(HexCell hexCell)
		{
			this.hexCell = hexCell;
			Index = hexCell.Index;
		}
	}

	public class SimulationCellContainer
	{
		//We've got this saving and loading with json serialization but for this specific instance,
		//this is far too slow and won't work. SimCell will need to be populated by primitives
		//and saved the same way we save HexCells. Perhaps things that care about what cell they are in
		//should just save a reference to its index, and find it on demand?? This might be better.
		public List<SimulationCell> SimulationCells { get; set; }

		public SimulationCellContainer() { }

		public SimulationCellContainer(HexCell[] hexCells)
		{
			SimulationCells = new List<SimulationCell>();
			for (int i = 0; i < hexCells.Length; i++)
			{
				SimulationCells.Add(new SimulationCell(hexCells[i]));
			}
		}

		public void Save(string mapName)
		{
			SaveUtilities.SerializeSave(this, Path.Combine(SaveUtilities.GetSimCellsPath(mapName), "SimCells.json"));
		}
	}
}
