using Game.GameMath;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Terrain
{
    public class MeshPolygonBounds : MonoBehaviour
    {
        public List<Transform> corners;

        public Polygon GetPolygon()
        {
            var v2s = new List<Vector2>();
            foreach (var corner in corners)
            {
                v2s.Add(new Vector2(corner.position.x, corner.position.z));
            }

            return new Polygon(v2s);
        }
    }
}