using System.Collections;
using Game.Data;
using Game.Simulation;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class IncidentActionTests : Editor
{
    // A Test behaves as an ordinary method
    /*
    [Test]
    public void IncidentActionTestsSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator IncidentActionTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
    */
    
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
}
