using Game.Factions;
using Game.WorldGeneration;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game.Visuals
{
	public class TileVisual : MonoBehaviour
	{
		[SerializeField]
		GameObject terrainObjectRoot;

		[SerializeField]
		Transform propContainerRoot;

		[SerializeField]
		Renderer factionColorRenderer;

		public Tile tile;

		private List<PropPlaceholder> props;

		public Renderer Renderer => terrainObjectRoot.GetComponent<Renderer>();

		private void Start()
		{
			props = propContainerRoot.GetComponentsInChildren<PropPlaceholder>().ToList();

			foreach(var prop in props)
			{
				prop.ReplaceWithProp(tile);
			}
		}

		public void UpdateFactionController()
		{
			var colorMat = new Material(factionColorRenderer.sharedMaterial);

			if (tile.controller != null)
			{
				var color = new Color(tile.controller.color.r, tile.controller.color.g, tile.controller.color.b, 0.5f);
				colorMat.color = color;
			}
			else
			{
				colorMat.color = Color.clear;
			}

			factionColorRenderer.sharedMaterial = colorMat;
		}
	}
}