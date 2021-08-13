using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonStats
{
	public int strength;
	public int agility;
	public int constitution;
	public int intelligence;
	public int wisdom;
	public int charisma;
	public int luck;

	public int strengthMod => (strength - 10)/2;
	public int agilityMod => (agility - 10) / 2;
	public int constitutionMod => (constitution - 10) / 2;
	public int intelligenceMod => (intelligence - 10) / 2;
	public int wisdomMod => (wisdom - 10) / 2;
	public int charismaMod => (charisma - 10) / 2;
	public int luckMod => (luck - 10) / 2;

	public PersonStats(int strength, int agility, int constitution, int intelligence, int wisdom, int charisma, int luck)
	{
		this.strength = strength;
		this.agility = agility;
		this.constitution = constitution;
		this.intelligence = intelligence;
		this.wisdom = wisdom;
		this.charisma = charisma;
		this.luck = luck;
	}
}
