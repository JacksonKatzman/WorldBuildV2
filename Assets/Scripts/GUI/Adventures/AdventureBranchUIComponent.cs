using Game.Simulation;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.GUI.Wiki
{
	public class AdventureBranchUIComponent : AdventureUIComponent
	{
		public GameObject buttonPrefab;
		public Transform buttonRoot;
		private List<AdventurePathUIComponent> pathButtons;

		public override void BuildUIComponents(IAdventureComponent component)
		{
			pathButtons = new List<AdventurePathUIComponent>();

			var branchingComponent = component as AdventureBranchingComponent;
			foreach(var path in branchingComponent.paths)
			{
				var pathButton = Instantiate(buttonPrefab, buttonRoot).GetComponent<AdventurePathUIComponent>();
				pathButton.Setup(path.components[0].ComponentID, path.pathTitle);
				pathButtons.Add(pathButton);
			}
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			
		}

		public override void ReplaceTextPlaceholders(List<IAdventureContextCriteria> contexts)
		{
			foreach(var button in pathButtons)
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
