using Game.GUI.Wiki;
using Game.Simulation;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.GUI.Adventures
{
	public class AdventureBranchUIComponent : AdventureUIComponent<AdventureBranchingComponent>
	{
		public GameObject buttonPrefab;
		public Transform buttonRoot;
		private List<AdventurePathUIComponent> pathButtons;

		protected override List<TMP_Text> AssociatedTexts => new List<TMP_Text>();

		public override void BuildUIComponents(AdventureBranchingComponent component)
		{
			pathButtons = new List<AdventurePathUIComponent>();

			var branchingComponent = component;
			foreach (var path in branchingComponent.paths)
			{
				var pathButton = Instantiate(buttonPrefab, buttonRoot).GetComponent<AdventurePathUIComponent>();
				pathButton.Setup(path.components[0].ComponentID, path.pathTitle);
				pathButtons.Add(pathButton);
			}
		}

		public override void OnPointerClick(PointerEventData eventData)
		{

		}

		public override void ReplaceTextPlaceholders(List<IAdventureContextRetriever> contexts)
		{
			foreach (var button in pathButtons)
			{
				button.ReplaceTextPlaceholders(contexts);
			}
		}

		protected override void ToggleElements()
		{
			//throw new NotImplementedException();
		}
	}
}
