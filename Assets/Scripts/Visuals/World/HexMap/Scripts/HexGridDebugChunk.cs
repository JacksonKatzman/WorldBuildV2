namespace Game.Terrain
{
    public class HexGridDebugChunk : HexGridTerrainChunk
    {
		public void TestInitMeshes()
		{
			terrain.InitMesh();
			rivers.InitMesh();
			roads.InitMesh();
			water.InitMesh();
			waterShore.InitMesh();
			estuaries.InitMesh();
			features.walls.InitMesh();
		}

		public override void Triangulate()
		{
			terrain.Clear();
			rivers.Clear();
			roads.Clear();
			water.Clear();
			waterShore.Clear();
			estuaries.Clear();

			Triangulate(cells[0]);

			terrain.Apply();
			rivers.Apply();
			roads.Apply();
			water.Apply();
			waterShore.Apply();
			estuaries.Apply();
		}
	}
}