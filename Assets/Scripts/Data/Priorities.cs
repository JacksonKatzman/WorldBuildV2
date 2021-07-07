using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priorities
{
    public int militaryScore;
    public int infrastructureScore;
    public int mercantileScore;
    public int politicalScore;
    public int expansionScore;

	public Priorities(int militaryScore, int infrastructureScore, int mercantileScore, int politicalScore, int expansionScore)
	{
		this.militaryScore = Mathf.Clamp(militaryScore, 0, 20);
		this.infrastructureScore = Mathf.Clamp(infrastructureScore, 0, 20);
		this.mercantileScore = Mathf.Clamp(mercantileScore, 0, 20);
		this.politicalScore = Mathf.Clamp(politicalScore, 0, 20);
		this.expansionScore = Mathf.Clamp(expansionScore, 0, 20);
	}

	public static int CompareScores(Priorities staticScore, Priorities variableScore)
	{
		var comparedMilitary = Mathf.Abs(staticScore.militaryScore - variableScore.militaryScore);
		var comparedInfrastructure = Mathf.Abs(staticScore.infrastructureScore - variableScore.infrastructureScore);
		var comparedMercantile = Mathf.Abs(staticScore.mercantileScore - variableScore.mercantileScore);
		var comparedPolitical = Mathf.Abs(staticScore.politicalScore - variableScore.politicalScore);
		var comparedExpansion = Mathf.Abs(staticScore.expansionScore - variableScore.expansionScore);

		return comparedMilitary + comparedInfrastructure + comparedMercantile + comparedPolitical + comparedExpansion;
	}

	public static Priorities operator +(Priorities a, Priorities b)
	{
		var comparedMilitary = a.militaryScore + b.militaryScore;
		var comparedInfrastructure = a.infrastructureScore + b.infrastructureScore;
		var comparedMercantile = a.mercantileScore + b.mercantileScore;
		var comparedPolitical = a.politicalScore + b.politicalScore;
		var comparedExpansion = a.expansionScore + b.expansionScore;

		return new Priorities(comparedMilitary, comparedInfrastructure, comparedMercantile, comparedPolitical, comparedExpansion);
	}

	public static Priorities operator -(Priorities a, Priorities b)
	{
		var comparedMilitary = a.militaryScore - b.militaryScore;
		var comparedInfrastructure = a.infrastructureScore - b.infrastructureScore;
		var comparedMercantile = a.mercantileScore - b.mercantileScore;
		var comparedPolitical = a.politicalScore - b.politicalScore;
		var comparedExpansion = a.expansionScore - b.expansionScore;

		return new Priorities(comparedMilitary, comparedInfrastructure, comparedMercantile, comparedPolitical, comparedExpansion);
	}

	public override string ToString()
	{
		return $"MI: {militaryScore} / IN: {infrastructureScore} / ME: {mercantileScore} / PO: {politicalScore} / EX: {expansionScore}";
	}
}

public class ActionKey
{
	public string fuctionName;
	public int influenceRequirement;
	public Priorities score;

	public ActionKey(string fuctionName, int influenceRequirement, Priorities score)
	{
		this.fuctionName = fuctionName;
		this.influenceRequirement = influenceRequirement;
		this.score = score;
	}
}
