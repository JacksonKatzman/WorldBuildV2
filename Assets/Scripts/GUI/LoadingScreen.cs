using Game.Simulation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Game.GUI
{
	public class LoadingScreen : MonoBehaviour
	{
		[SerializeField]
		private CanvasGroup loadingScreenCanvasGroup;
		[SerializeField]
		private TMP_Text loadingScreenMainText;
		[SerializeField]
		private TMP_Text loadingScreenStatusText;
		[SerializeField, Range(1.0f, 10.0f)]
		private float statusCycleTime;

		public List<string> statusStrings;
		private Queue<string> statusQueue;
		private IEnumerator statusCoroutine;

		private void Awake()
		{
			FillStatusQueue();
			EventManager.Instance.AddEventHandler<ShowLoadingScreenEvent>(OnShowLoadingScreenEvent);
			EventManager.Instance.AddEventHandler<HideLoadingScreenEvent>(OnHideLoadingScreenEvent);
		}

		public void TestShowLoadingScreen()
		{
			statusCoroutine = CycleStatus();
			ToggleCanvasGroup(loadingScreenCanvasGroup, true);
		}

		public void TestHideLoadingScreen()
		{
			ToggleCanvasGroup(loadingScreenCanvasGroup, false);
			StopCoroutine(statusCoroutine);
		}

		private void OnShowLoadingScreenEvent(ShowLoadingScreenEvent gameEvent)
		{
			loadingScreenMainText.text = gameEvent.loadingScreenMainText;
			statusCoroutine = CycleStatus();
			StartCoroutine(statusCoroutine);
			ToggleCanvasGroup(loadingScreenCanvasGroup, true);
		}

		private void OnHideLoadingScreenEvent(HideLoadingScreenEvent gameEvent)
		{
			ToggleCanvasGroup(loadingScreenCanvasGroup, false);
			StopCoroutine(statusCoroutine);
		}

		private void ToggleCanvasGroup(CanvasGroup group, bool on)
		{
			group.alpha = on ? 1 : 0;
			group.interactable = on;
			group.blocksRaycasts = on;
		}

		private void FillStatusQueue()
		{
			var copy = new List<string>(statusStrings);
			var count = copy.Count;
			var last = count - 1;
			for (var i = 0; i < last; ++i)
			{
				var r = UnityEngine.Random.Range(i, count);
				var tmp = copy[i];
				copy[i] = copy[r];
				copy[r] = tmp;
			}

			statusQueue = new Queue<string>(copy);
		}

		private IEnumerator CycleStatus()
		{
			while (true)
			{
				loadingScreenStatusText.text = statusQueue.Dequeue();
				if (statusQueue.Count <= 0)
				{
					FillStatusQueue();
				}
				yield return new WaitForSeconds(statusCycleTime);
			}
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
			EventManager.Instance.RemoveEventHandler<ShowLoadingScreenEvent>(OnShowLoadingScreenEvent);
			EventManager.Instance.RemoveEventHandler<HideLoadingScreenEvent>(OnHideLoadingScreenEvent);
		}
	}
}
