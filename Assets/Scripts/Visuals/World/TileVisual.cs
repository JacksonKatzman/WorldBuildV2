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
		Transform landmarkPropContainerRoot;

		[SerializeField]
		Renderer factionColorRenderer;

		public Tile tile;

		private List<PropPlaceholder> props;
		private List<LandmarkPropPlaceholder> landmarkProps;

		public Renderer Renderer => terrainObjectRoot.GetComponent<Renderer>();

		public void UpdateVisuals()
		{
			UpdateFactionController();
			UpdateProps();
		}

		public void Initialize()
		{
			props = propContainerRoot.GetComponentsInChildren<PropPlaceholder>().ToList();
			landmarkProps = landmarkPropContainerRoot.GetComponentsInChildren<LandmarkPropPlaceholder>().ToList();

			foreach (var prop in props)
			{
				prop.ReplaceWithProp(tile);
				prop.HidePlaceholder();
			}

			foreach(var prop in landmarkProps)
			{
				prop.HidePlaceholder();
			}
		}

		private void UpdateFactionController()
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

		private void UpdateProps()
		{
			foreach(var p in landmarkProps)
			{
				if(p.landmark != null && !tile.landmarks.Contains(p.landmark))
				{
					p.Reset();
				}
			}

			foreach(var landmark in tile.landmarks)
			{
				var placeholder = landmarkProps.Find(x => x.landmark == landmark);
				var empties = landmarkProps.Where(x => x.landmark == null).ToList();
				if (placeholder == null && empties.Count > 0)
				{
					SimRandom.RandomEntryFromList(empties).ReplaceWithProp(landmark);
				}
			}
		}
	}
}