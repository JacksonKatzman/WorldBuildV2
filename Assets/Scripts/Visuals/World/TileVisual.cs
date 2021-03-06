using Game.Factions;
using Game.WorldGeneration;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Game.Enums;

namespace Game.Visuals
{
	public class TileVisual : MonoBehaviour
	{
		[SerializeField]
		GameObject terrainObjectRoot;

		[SerializeField]
		Transform tileRoot;

		Transform propContainerRoot => tileRoot.GetChild(0);

		Transform landmarkPropContainerRoot => tileRoot.GetChild(1);

		[SerializeField]
		Renderer factionColorRenderer;

		[SerializeField]
		List<TileBisectorContainer> riverPieces;

		[SerializeField]
		List<TileBisectorContainer> roadPieces;

		[SerializeField]
		public DebugDirections debugRiverDirections;

		public Tile tile;

		private List<PropPlaceholder> props;
		private List<LandmarkPropPlaceholder> landmarkProps;

		private int lastRotation;

		public Renderer Renderer => terrainObjectRoot.GetComponent<Renderer>();

		public void UpdateVisuals()
		{
			UpdateRoads();
			UpdateFactionController();
			UpdateProps();
		}

		public void Initialize()
		{
			debugRiverDirections.directions = tile.riverDirections;
			UpdateRivers();

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

		private void UpdateRoads()
		{
			if(roadPieces.Count > 0 && tile.roadDirections.Count > 1)
			{
				TileBisectType bisectType = TileBisectType.NONE;
				int rotationDegrees = 0;
				if (tile.riverDirections.Count > 0)
				{
					bisectType = TileBisectType.DOUBLE;
					if(tile.roadDirections.Contains(Direction.EAST))
					{
						rotationDegrees = 90;
					}
				}
				else if (tile.roadDirections.Count == 2)
				{
					//curve or straight
					if ((tile.roadDirections.Contains(Direction.SOUTH) && tile.roadDirections.Contains(Direction.NORTH)) || (tile.roadDirections.Contains(Direction.WEST) && tile.roadDirections.Contains(Direction.EAST)))
					{
						//straight
						bisectType = TileBisectType.STRAIGHT;
						if (tile.roadDirections.Contains(Direction.NORTH))
						{
							rotationDegrees = 90;
						}
					}
					else
					{
						//curve
						bisectType = TileBisectType.CURVE;
						if (tile.roadDirections.Contains(Direction.SOUTH) && tile.roadDirections.Contains(Direction.EAST))
						{
							rotationDegrees = 270;
						}
						else if (tile.roadDirections.Contains(Direction.WEST) && tile.roadDirections.Contains(Direction.NORTH))
						{
							rotationDegrees = 90;
						}
						else if (tile.roadDirections.Contains(Direction.EAST) && tile.roadDirections.Contains(Direction.NORTH))
						{
							rotationDegrees = 180;
						}
					}
				}
				else if (tile.roadDirections.Count == 3)
				{
					bisectType = TileBisectType.THREEWAY;
					if (!tile.roadDirections.Contains(Direction.EAST))
					{
						rotationDegrees = 90;
					}
					else if (!tile.roadDirections.Contains(Direction.SOUTH))
					{
						rotationDegrees = 180;
					}
					else if (!tile.roadDirections.Contains(Direction.WEST))
					{
						rotationDegrees = 270;
					}
				}
				else if (tile.roadDirections.Count == 4)
				{
					bisectType = TileBisectType.FOURWAY;
				}

				tileRoot.gameObject.SetActive(false);
				tileRoot = roadPieces.Find(x => x.type == bisectType).gameObject.transform;
				tileRoot.gameObject.SetActive(true);
				if (lastRotation != rotationDegrees)
				{
					tileRoot.Rotate(0, rotationDegrees, 0);
					lastRotation = rotationDegrees;
				}
			}
		}

		private void UpdateRivers()
		{
			if(riverPieces.Count > 0 && tile.riverDirections.Count > 1)
			{
				TileBisectType bisectType = TileBisectType.NONE;
				int rotationDegrees = 0;

				if(tile.riverDirections.Count == 2)
				{
					//curve or straight
					if((tile.riverDirections.Contains(Direction.SOUTH) && tile.riverDirections.Contains(Direction.NORTH)) || (tile.riverDirections.Contains(Direction.WEST) && tile.riverDirections.Contains(Direction.EAST)))
					{
						//straight
						bisectType = TileBisectType.STRAIGHT;
						if(tile.riverDirections.Contains(Direction.NORTH))
						{
							rotationDegrees = 90;
						}
					}
					else
					{
						//curve
						bisectType = TileBisectType.CURVE;
						if(tile.riverDirections.Contains(Direction.SOUTH) && tile.riverDirections.Contains(Direction.EAST))
						{
							rotationDegrees = 270;
						}
						else if (tile.riverDirections.Contains(Direction.WEST) && tile.riverDirections.Contains(Direction.NORTH))
						{
							rotationDegrees = 90;
						}
						else if (tile.riverDirections.Contains(Direction.EAST) && tile.riverDirections.Contains(Direction.NORTH))
						{
							rotationDegrees = 180;
						}
					}
				}
				else if(tile.riverDirections.Count == 3)
				{
					bisectType = TileBisectType.THREEWAY;
					if(!tile.riverDirections.Contains(Direction.EAST))
					{
						rotationDegrees = 90;
					}
					else if (!tile.riverDirections.Contains(Direction.SOUTH))
					{
						rotationDegrees = 180;
					}
					else if(!tile.riverDirections.Contains(Direction.WEST))
					{
						rotationDegrees = 270;
					}
				}
				else if(tile.riverDirections.Count == 4)
				{
					bisectType = TileBisectType.FOURWAY;
				}

				tileRoot = riverPieces.Find(x => x.type == bisectType).gameObject.transform;
				tileRoot.gameObject.SetActive(true);
				tileRoot.Rotate(0, rotationDegrees, 0);

				foreach(var container in riverPieces)
				{
					if(container.type != bisectType)
					{
						container.gameObject.SetActive(false);
					}
				}
			}
		}
	}

	public class BisectorAnglePair
	{
		TileBisectType type;
		int angle;

		public BisectorAnglePair(TileBisectType type, int angle)
		{
			this.type = type;
			this.angle = angle;
		}
	}

	[System.Serializable]
	public class TileBisectorContainer
	{
		public TileBisectType type;
		public GameObject gameObject;
	}

	[System.Serializable]
	public class DebugDirections
	{
		public List<Direction> directions;
	}
}