﻿using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Simulation
{
	public class AdventureTextComponent : AdventureComponent, IAdventureTextComponent
	{
		public AdventureComponentTextField textField = new AdventureComponentTextField();

		public string Text => textField.text;
	}
}
