using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScore
{
    public int militaryScore;
    public int infrastructureScore;
    public int mercantileScore;
    public int politicalScore;
    public int expansionScore;

	public ActionScore(int militaryScore, int infrastructureScore, int mercantileScore, int politicalScore, int expansionScore)
	{
		this.militaryScore = Mathf.Clamp(militaryScore, 0, 10);
		this.infrastructureScore = Mathf.Clamp(infrastructureScore, 0, 10);
		this.mercantileScore = Mathf.Clamp(mercantileScore, 0, 10);
		this.politicalScore = Mathf.Clamp(politicalScore, 0, 10);
		this.expansionScore = Mathf.Clamp(expansionScore, 0, 10);
	}

	public static int CompareScores(ActionScore staticScore, ActionScore variableScore)
	{
		var comparedMilitary = Mathf.Abs(staticScore.militaryScore - variableScore.militaryScore);
		var comparedInfrastructure = Mathf.Abs(staticScore.infrastructureScore - variableScore.infrastructureScore);
		var comparedMercantile = Mathf.Abs(staticScore.mercantileScore - variableScore.mercantileScore);
		var comparedPolitical = Mathf.Abs(staticScore.politicalScore - variableScore.politicalScore);
		var comparedExpansion = Mathf.Abs(staticScore.expansionScore - variableScore.expansionScore);

		return comparedMilitary + comparedInfrastructure + comparedMercantile + comparedPolitical + comparedExpansion;
	}
}

public class ActionKey
{
	public string fuctionName;
	public int influenceRequirement;
	public ActionScore score;

	public ActionKey(string fuctionName, int influenceRequirement, ActionScore score)
	{
		this.fuctionName = fuctionName;
		this.influenceRequirement = influenceRequirement;
		this.score = score;
	}
}
