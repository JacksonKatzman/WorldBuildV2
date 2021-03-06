using Game.Enums;
using System.Collections;
using UnityEngine;

namespace Game.Combat
{
	[System.Serializable]
	public class DamageValue
	{
		public int numDice;
		public int maxRoll;
		public int plusFlat;
		public DamageType damageType;

		public DamageValue(int numDice, int maxRoll, int plusFlat, DamageType damageType)
		{
			this.numDice = numDice;
			this.maxRoll = maxRoll;
			this.plusFlat = plusFlat;
			this.damageType = damageType;
		}

		public int RollDamage()
		{
			return SimRandom.RollXDY(numDice, maxRoll) + plusFlat;
		}
	}
}