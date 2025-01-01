using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialScenario", menuName = "Tutorial/Tutorial Scenario", order = 1)]
public class TutorialScenario : ScriptableObject
{
    [Tooltip("Unique identifier for the scenario.")]
    public int scenarioIndex;

    [Tooltip("Initial pool of cards for this scenario.")]
    public List<Card> tutorialPool;

    [Tooltip("Sequence of cards the bot will draw.")]
    public List<Card> tutorialBotDrawSequence;

    [Tooltip("Sequence of cards the player will draw.")]
    public List<Card> tutorialPlayerDrawSequence;
}