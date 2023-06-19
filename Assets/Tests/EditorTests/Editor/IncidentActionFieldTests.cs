using Game.Incidents;
using Game.Simulation;
using NUnit.Framework;
using UnityEditor;

public class IncidentActionFieldTests : Editor
{
    [SetUp]
    public void SetUp()
    {
        var current = new IncidentContextDictionary();
        var all = new IncidentContextDictionary();
        ContextDictionaryProvider.SetContextsProviders(() => current, () => all);
        ContextDictionaryProvider.SetNextID(1);
    }

    [TearDown]
    public void TearDown()
    {
        ContextDictionaryProvider.SetContextsProviders(() => null, () => null);
        ContextDictionaryProvider.SetNextID(1);
    }

    [Test]
    public void ContextualIncidentActionField_RetrieveValueFromPrevious()
	{
        var previousFaction = new Faction();
        var previousFactionField = new ContextualIncidentActionField<Faction>() { value = previousFaction, ActionFieldID = 1 };

        var factionField = new ContextualIncidentActionField<Faction>() { PreviousFieldID = 1 };
	}
}
