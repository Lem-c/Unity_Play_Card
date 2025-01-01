using System.Collections.Generic;
using UnityEngine;

public class TutorialScenarioFactory : MonoBehaviour
{
    [SerializeField]
    private List<TutorialScenario> tutorialScenarios;

    // Dictionary for quick lookup by scenarioIndex
    private Dictionary<int, TutorialScenario> scenarioDictionary;

    private void Awake()
    {
        // Initialize the dictionary
        scenarioDictionary = new Dictionary<int, TutorialScenario>();

        foreach (var scenario in tutorialScenarios)
        {
            if (!scenarioDictionary.ContainsKey(scenario.scenarioIndex))
            {
                scenarioDictionary.Add(scenario.scenarioIndex, scenario);
            }
            else
            {
                Debug.LogWarning($"Duplicate scenarioIndex {scenario.scenarioIndex} found in {scenario.name}. Ignoring duplicates.");
            }
        }
    }

    /// <summary>
    /// Retrieves a TutorialScenario based on the scenarioIndex.
    /// </summary>
    /// <param name="scenarioIndex">The index of the desired scenario.</param>
    /// <returns>The corresponding TutorialScenario or null if not found.</returns>
    public TutorialScenario GetScenario(int scenarioIndex)
    {
        if (scenarioDictionary.TryGetValue(scenarioIndex, out TutorialScenario scenario))
        {
            return scenario;
        }
        else
        {
            Debug.LogWarning($"TutorialScenario with index {scenarioIndex} not found.");
            return null;
        }
    }
}
