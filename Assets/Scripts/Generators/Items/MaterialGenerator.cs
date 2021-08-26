using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Game.Generators.Items
{
	public class MaterialGenerator
	{
		private List<Material> materials;

		public MaterialGenerator()
		{
			LoadPresetMaterials();
		}

		public Material GetRandomMaterialByUse(MaterialUse use, bool allowCreation)
		{
			var query =
				from mat in materials
				where mat.uses.Contains(use)
				select mat;
			var materialList = query.ToList();

			var creationChance = allowCreation ? 1 : 0;
			var randomIndex = SimRandom.RandomRange(0, materialList.Count + creationChance);
			
			if(materialList.Count - randomIndex == 0)
			{
				//make new mat
				return GenerateNewMaterialWithUses(new List<MaterialUse> { use });
			}
			else
			{
				//use existing mat
				return materialList[randomIndex];
			}
		}

		public Material GenerateNewMaterialWithUses(List<MaterialUse> uses)
		{
			var name = NameGenerator.GenerateMaterialName();
			var mat = new Material(name, uses, SimRandom.RandomFloat01());
			materials.Add(mat);
			return mat;
		}

		private void LoadPresetMaterials()
		{
			materials = new List<Material>();

			var text = DataManager.Instance.materialInfo;

			string[] mats = text.text.Split('\n');

			foreach (string mat in mats)
			{
				string[] splitValues = mat.Split(',');
				var name = splitValues[0];
				var rarity = float.Parse(splitValues[2]);

				var uses = new List<MaterialUse>();
				var useValues = splitValues[1].Split('/');
				foreach(var useString in useValues)
				{
					var use = (MaterialUse)Enum.Parse(typeof(MaterialUse), useString);
					uses.Add(use);
				}

				materials.Add(new Material(name, uses, rarity));
			}
		}
	}
}