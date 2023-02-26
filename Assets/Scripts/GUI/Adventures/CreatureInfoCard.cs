using Game.Enums;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GUI
{
	public class CreatureInfoCard : SerializedMonoBehaviour
	{
		public Dictionary<DamageType, GameObject> damageVulnerabilities = new Dictionary<DamageType, GameObject>()
		{
			{DamageType.SLASHING, null }, {DamageType.PIERCING, null }, {DamageType.BLUDGEONING, null },
			{DamageType.POISON, null }, {DamageType.ACID, null }, {DamageType.FIRE, null },
			{DamageType.COLD, null }, {DamageType.RADIANT, null }, {DamageType.NECROTIC, null },
			{DamageType.THUNDER, null }, {DamageType.LIGHTNING, null }, {DamageType.FORCE, null },
			{DamageType.PSYCHIC, null }
		};
		public Dictionary<DamageType, GameObject> damageResistances = new Dictionary<DamageType, GameObject>()
		{
			{DamageType.SLASHING, null }, {DamageType.PIERCING, null }, {DamageType.BLUDGEONING, null },
			{DamageType.POISON, null }, {DamageType.ACID, null }, {DamageType.FIRE, null },
			{DamageType.COLD, null }, {DamageType.RADIANT, null }, {DamageType.NECROTIC, null },
			{DamageType.THUNDER, null }, {DamageType.LIGHTNING, null }, {DamageType.FORCE, null },
			{DamageType.PSYCHIC, null }
		};
		public Dictionary<DamageType, GameObject> damageImmunities = new Dictionary<DamageType, GameObject>()
		{
			{DamageType.SLASHING, null }, {DamageType.PIERCING, null }, {DamageType.BLUDGEONING, null },
			{DamageType.POISON, null }, {DamageType.ACID, null }, {DamageType.FIRE, null },
			{DamageType.COLD, null }, {DamageType.RADIANT, null }, {DamageType.NECROTIC, null },
			{DamageType.THUNDER, null }, {DamageType.LIGHTNING, null }, {DamageType.FORCE, null },
			{DamageType.PSYCHIC, null }
		};
		public Dictionary<ConditionType, GameObject> conditionImmunities = new Dictionary<ConditionType, GameObject>()
		{
			{ConditionType.BLINDED, null }, {ConditionType.CHARMED, null }, {ConditionType.DEAFENED, null },
			{ConditionType.FRIGHTENED, null }, {ConditionType.GRAPPLED, null }, {ConditionType.INCAPACITATED, null },
			{ConditionType.INVISIBLE, null }, {ConditionType.PARALYZED, null }, {ConditionType.PETRIFIED, null },
			{ConditionType.POISONED, null }, {ConditionType.PRONE, null }, {ConditionType.RESTRAINED, null },
			{ConditionType.STUNNED, null }, {ConditionType.UNCONSCIOUS, null }, {ConditionType.EXHAUSTION, null }
		};
	}
}
