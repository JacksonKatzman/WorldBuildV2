using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Visuals.Hex
{
	public class HexMapEditor : MonoBehaviour
	{
		[SerializeField]
		Camera mainCamera;

		public Color[] colors;

		public HexGrid hexGrid;

		private Color activeColor;

		void Awake()
		{
			SelectColor(0);
		}

		void Update()
		{
			if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
			{
				HandleInput();
			}
		}

		void HandleInput()
		{
			Ray inputRay = mainCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(inputRay, out hit))
			{
				hexGrid.ColorCell(hit.point, activeColor);
			}
		}

		public void SelectColor(int index)
		{
			activeColor = colors[index];
		}
	}
}
