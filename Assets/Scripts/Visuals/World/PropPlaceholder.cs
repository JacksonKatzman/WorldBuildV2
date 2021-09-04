﻿using Game.WorldGeneration;
using System.Collections;
using UnityEngine;

namespace Game.Visuals
{
	public class PropPlaceholder : MonoBehaviour
	{
		[SerializeField]
		protected Renderer placeholderRenderer;

		[SerializeField]
		protected GameObject currentProp;
		public virtual void ReplaceWithProp(Tile tile)
		{
			if(currentProp != null)
			{
				Destroy(currentProp);
			}

			if (tile.biome.propDictionary.Count > 0)
			{
				placeholderRenderer.enabled = false;

				var prefab = SimRandom.RandomEntryFromWeightedDictionary(tile.biome.propDictionary);
				currentProp = Instantiate(prefab, transform);
			}
		}
	}
}