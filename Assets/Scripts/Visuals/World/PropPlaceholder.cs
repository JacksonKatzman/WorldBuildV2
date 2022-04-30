using Game.WorldGeneration;
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
				var randomRotation = Quaternion.Euler(0, SimRandom.RandomRange(0, 360), 0);
				currentProp = Instantiate(prefab, transform.position, randomRotation, transform);
			}
		}

		public void HidePlaceholder()
		{
			placeholderRenderer.enabled = false;
		}
	}
}