using Game.WorldGeneration;
using System.Collections;
using UnityEngine;

namespace Game.Visuals
{
	public class LandmarkPropPlaceholder : MonoBehaviour
	{
		[SerializeField]
		protected Renderer placeholderRenderer;

		[SerializeField]
		protected GameObject currentProp;

		public Landmark landmark;

		public void ReplaceWithProp(Landmark landmark)
		{
			if (currentProp != null)
			{
				Destroy(currentProp);
			}

			this.landmark = landmark;
			var prefab = SimRandom.RandomEntryFromList(DataManager.Instance.landmarkProps[landmark.GetType()]);

			if(prefab != null)
			{
				placeholderRenderer.enabled = false;
				var randomRotation = Quaternion.Euler(0, SimRandom.RandomRange(0, 360), 0);
				currentProp = Instantiate(prefab, transform.position, randomRotation, transform);
			}	
		}

		public void HidePlaceholder()
		{
			placeholderRenderer.enabled = false;
		}

		public void Reset()
		{
			if (currentProp != null)
			{
				Destroy(currentProp);
			}

			landmark = null;
		}
	}
}