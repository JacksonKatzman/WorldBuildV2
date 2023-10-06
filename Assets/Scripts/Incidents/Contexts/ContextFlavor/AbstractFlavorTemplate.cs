using Game.Enums;
using Sirenix.OdinInspector;

namespace Game.Incidents
{
	abstract public class AbstractFlavorTemplate : SerializedScriptableObject, IFlavorTemplate
	{
		public OrganizationType permType;

		[PropertyRange(-10,10)]
		public int lawfulChaoticThreshold;
		[PropertyRange(-10, 10)]
		public int goodEvilThreshold;

		public string flavor;

		public bool MeetsRequirements(OrganizationType perms, int goodEvilValue, int lawfulChaoticValue)
		{
			if(permType != OrganizationType.OTHER && perms != permType)
			{
				return false;
			}

			return EvaluateThreshold(goodEvilValue, goodEvilThreshold) && EvaluateThreshold(lawfulChaoticValue, lawfulChaoticThreshold);
		}

		private bool EvaluateThreshold(int input, int threshold)
		{
			if(threshold == 0)
			{
				return true;
			}
			else if(threshold > 0 && input >threshold)
			{
				return true;
			}
			else if(threshold < 0 && input < threshold)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}

//a collection of the possible grand actions/feats a character might take.
//things like slaying a dragon or establishing an orphanage
//could be evil things too obv

//need to take in both the alignment of the character and the PERMS to decide which kind of action they are most likely to take
//need a way of storing the actions as mad lib strings where they arent all divided up into
//alignment x perm sections cuz that would create ALOT of divisions. 

//At top level we have the PERMs, including "Other". Dictionary<OrganizationType, GradActionSubCollection> (name pending) ScoredFlavorStringCollection?
//GrandActionSubCollections would be a list of objects which include the good/evil score of the action,
//the lawful/chaotic score of the action, and the action string itself.

//For example: Slaying a monster who was attacking a village would have:
//	-Good/Evil score of +5 because protecting innocents is good
//	-Lawful/Chaotic score somewhere near 0 since the why probably doesnt matter as much
//	-A madlib style string description of the event.

//As part of the params for finding a GrandAction for a character to take, we send the character as a context object.
//We can then use their lawful/chaotic score and their good/evil score to determine what they might do.
//We first check their PERM score to see what they care about most and pick from that list.
//We then try to find an action which most closely meets their l/c + g/e scores.
//Would also be good to then modify their l/c + g/e afterwards to match their deeds, either on the spot, or by returning something
//that another fn could use to make the modifications.

//Maybe we check G/E first and get all actions +/- some value to give some options. Maybe the modulus increases/decreases by how chaotic/lawful they are? Or by some other factor
//Better to do that than just have it be +/- 1 all the time.
//THEN we pair it down again to find only the ones from that collection that are also similar in their L/C.
//And if we find nothing, then we need to increase the modulus by one and go again until we succeed.
//So that probably means that unless its an extreme case, the modulus starts at 0 for the G/E and increases on failures.
//And the modulus for the L/C is never more than like 1 or 2? Cuz im not currently sure how else wed handle a fail case for flavor. A set of benign, default returns?

//Might not make sense to do this for most things. I think maybe instead we just do this for reasons, and do most things in incidents so we can do more things with the contexts.

//INSTEAD lets do the flavors inside the incidents themselves. Similar to the list of IncidentActions we would have IncidentFlavors.
//In each of those you could select a template which would require the l/c + g/e and perms used, and would contain a set of context fields
//to be grabbed in the same way the incident actions do. So you could say:
//Pick a REASON_1 template which indicates only the primary subject would be included, eg: because {0} was jealous.
//OR
//Pick a REASON_2 template, which indicates a primary subject AND one secondary subject, eg: because {0} killed {1}'s son, or {1} killed {0}'s son, depending on the madlib.
//Each template or template type would have many many madlibs living in a list that it could pick from at random. Each one would have to fit the given template though.
//Might need to be even more specific with the template types to illustrate whether the primary is acting or being acted upon, such as in the example above.
//Need to decide on a way of storing these data collections, as well as grouping them at runtime to be looked up.