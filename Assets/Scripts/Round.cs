using System.Collections.Generic;
using UnityEngine;

public class Round
{
    private CardPoolManager cardPoolManager;
    private int playerCount;
    private List<Card> currentPool;

    // Tutorial-related fields
    private bool isTutorialMode;
    private List<Card> tutorialPool;              // The fixed pool for the scenario
    private List<Card> tutorialBotDrawSequence;   // The bot's fixed draw order
    private List<Card> tutorialPlayerDrawSequence;// The player's fixed draw order
    private int tutorialDrawIndex;                // Tracks how many times we've drawn cards so far

    private readonly double[] possibleValues = { 0.4, 0.5, 0.6, 0.7, 0.8 };

    public Round(CardPoolManager poolManager, int playerCount)
    {
        this.cardPoolManager = poolManager;
        this.playerCount = playerCount;
        SelectCardPool();
    }

    // Selects a random card pool (non-tutorial mode)
    private void SelectCardPool()
    {
        currentPool = playerCount == 4
            ? cardPoolManager.GetRandomThreePeoplePool()
            : cardPoolManager.GetRandomFivePeoplePool();
    }

    // Returns the current card pool
    public List<Card> GetCurrentPool()
    {
        return currentPool;
    }

    public void DrawCards(out Card drawnPlayerCard, out Card drawnBotCard, bool isChosenRound=false)
    {
        if (isTutorialMode)
        {
            // If in tutorial mode, override with fixed sequences
            DrawTutorialCards(out drawnPlayerCard, out drawnBotCard);
        }
        else
        {
            // Randomly draw from currentPool
            if (isChosenRound)
            {
                drawnPlayerCard = DrawPlayerWinningCard();

                drawnBotCard = DrawLoseCard();
            }
            else
            {
                // get possibility getting a black bean
                int index = Random.Range(0, possibleValues.Length);
                // assign player card
                drawnPlayerCard = GetBlackBeansByProbability(currentPool, possibleValues[index]);

                drawnBotCard = DrawCard();
            }
        }
    }

    private Card DrawCard()
    {
        if (currentPool.Count == 0)
        {
            Debug.LogWarning("No cards left in the pool!");
            return null;
        }

        int index = Random.Range(0, currentPool.Count);
        Card drawnCard = currentPool[index];
        currentPool.RemoveAt(index);
        return drawnCard;
    }

    /// <summary>
    /// Draws a card from the pool so that there is a <paramref name="targetProbability"/> chance 
    /// the drawn card is a "black bean" (IsBlackBean == true).
    /// 
    /// The chosen card is removed from the pool.
    /// 
    /// If there are no black bean cards but the random check falls, 
    /// we will fallback to a non-black-bean card (and vice versa).
    /// </summary>
    /// <param name="cards">The pool from which to draw.</param>
    /// <param name="targetProbability">The possibility getting a black bean</param>
    /// <returns>The drawn card, or null if pool is empty.</returns>
    public static Card GetBlackBeansByProbability(List<Card> cards, double targetProbability)
    {
        if (cards == null || cards.Count == 0)
        {
            return null;
        }

        // Partition
        var blackBeans = new List<Card>();
        var nonBlackBeans = new List<Card>();

        foreach (var card in cards)
        {
            if (card.IsBlackBean(cards))
                blackBeans.Add(card);
            else
                nonBlackBeans.Add(card);
        }

        float randomValue = Random.value;

        Card chosenCard;

        if (randomValue < 0.4f && blackBeans.Count > 0)
        {
            int index = Random.Range(0, blackBeans.Count);
            chosenCard = blackBeans[index];
        }
        // Otherwise: pick from non-black-bean cards
        else if (nonBlackBeans.Count > 0)
        {
            int index = Random.Range(0, nonBlackBeans.Count);
            chosenCard = nonBlackBeans[index];
        }
        // Fallback if the desired group is empty
        else if (blackBeans.Count > 0)
        {
            int index = Random.Range(0, blackBeans.Count);
            chosenCard = blackBeans[index];
        }
        else
        {
            // Should never happen if the pool has cards, but just in case
            return null;
        }

        // Remove the chosen card from the pool
        cards.Remove(chosenCard);

        return chosenCard;
    }

    private Card DrawLoseCard()
    {
        // Try to find a lose card (not black bean) in the pool
        for (int i = 0; i < currentPool.Count; i++)
        {
            if (!currentPool[i].IsBlackBean(currentPool))
            {
                Card loseCard = currentPool[i];
                currentPool.RemoveAt(i);
                return loseCard;
            }
        }

        // If no lose cards are left, fall back to a regular draw
        return DrawCard();
    }

    private Card DrawPlayerWinningCard()
    {
        // Try to find any black bean in the pool
        for (int i = 0; i < currentPool.Count; i++)
        {
            if (currentPool[i].IsBlackBean(currentPool))
            {
                Card blackBeanCard = currentPool[i];
                currentPool.RemoveAt(i);
                return blackBeanCard;
            }
        }

        // If no black beans are left, return a random regular draw
        return DrawCard();
    }

    // Refreshes the (random) card pool if needed
    public void RefreshPool()
    {
        if (!isTutorialMode)
        {
            SelectCardPool();
        }
        // If in tutorial mode, do nothing (tutorial pool is fixed)
    }

    /// <summary>
    /// Sets up a fixed tutorial pool and fixed draw sequences.
    /// After calling this, the Round will always draw cards from the known sequence
    /// instead of randomly.
    /// </summary>
    /// <param name="scenarioIndex">Which tutorial scenario (e.g., 1, 2, or 3).</param>
    [System.Obsolete("Method 'EnableTutorialMode' is deprecated, please use two-parameter method instead.", true)]
    public void EnableTutorialMode(int scenarioIndex)
    {
        isTutorialMode = true;
        tutorialDrawIndex = 0;  // Reset the draw index each time we enable

        // Depending on scenarioIndex, set the pool and sequences
        switch (scenarioIndex)
        {
            case 1:
                // First pool: [1, 2, 3, 4, 5, 6]
                // Draw bot: 5, 1, 2
                // Draw player: 6, 4, 3
                tutorialPool = new List<Card>
                {
                    new Card(1, "Hearts"), new Card(2, "Hearts"),
                    new Card(3, "Hearts"), new Card(4, "Hearts"),
                    new Card(5, "Hearts"), new Card(6, "Hearts")
                };
                tutorialBotDrawSequence = new List<Card>
                {
                    new Card(5, "Hearts"), new Card(1, "Hearts"), new Card(2, "Hearts")
                };
                tutorialPlayerDrawSequence = new List<Card>
                {
                    new Card(6, "Hearts"), new Card(4, "Hearts"), new Card(3, "Hearts")
                };
                break;

            case 2:
                // Second pool: [1, 2, 3, 4, 5, 6]
                // Draw bot: 1, 3, 5
                // Draw player: 3, 4, 6
                tutorialPool = new List<Card>
                {
                    new Card(1, "Hearts"), new Card(2, "Hearts"),
                    new Card(3, "Hearts"), new Card(4, "Hearts"),
                    new Card(5, "Hearts"), new Card(6, "Hearts")
                };
                tutorialBotDrawSequence = new List<Card>
                {
                    new Card(1, "Hearts"), new Card(3, "Hearts"), new Card(5, "Hearts")
                };
                tutorialPlayerDrawSequence = new List<Card>
                {
                    new Card(3, "Hearts"), new Card(4, "Hearts"), new Card(6, "Hearts")
                };
                break;

            case 3:
                // Third pool: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
                // Draw bot: 9, 7, 4, 1, 6
                // Draw player: 10, 8, 5, 2, 3
                tutorialPool = new List<Card>
                {
                    new Card(1, "Hearts"),  new Card(2, "Hearts"),
                    new Card(3, "Hearts"),  new Card(4, "Hearts"),
                    new Card(5, "Hearts"),  new Card(6, "Hearts"),
                    new Card(7, "Hearts"),  new Card(8, "Hearts"),
                    new Card(9, "Hearts"),  new Card(10, "Hearts")
                };
                tutorialBotDrawSequence = new List<Card>
                {
                    new Card(9, "Hearts"), new Card(7, "Hearts"),
                    new Card(4, "Hearts"), new Card(1, "Hearts"),
                    new Card(6, "Hearts")
                };
                tutorialPlayerDrawSequence = new List<Card>
                {
                    new Card(10, "Hearts"), new Card(8, "Hearts"),
                    new Card(5, "Hearts"),  new Card(2, "Hearts"),
                    new Card(3, "Hearts")
                };
                break;

            default:
                Debug.LogWarning("Falling back to normal pool.");
                isTutorialMode = false;
                return;
        }

        // Override current pool with the tutorial pool
        currentPool = new List<Card>(tutorialPool);
    }

    /// <summary>
    /// Sets up a tutorial mode based on the given scenario index.
    /// After calling this, the Round will always draw cards from the known sequence
    /// instead of randomly.
    /// </summary>
    /// <param name="scenarioIndex">Which tutorial scenario (e.g., 1, 2, or 3).</param>
    public void EnableTutorialMode(TutorialScenarioFactory tutorialScenarioFactory, int scenarioIndex)
    {
        // Retrieve the scenario from the factory
        TutorialScenario scenario = tutorialScenarioFactory.GetScenario(scenarioIndex);

        if (scenario == null)
        {
            Debug.LogWarning("Falling back to normal pool.");
            isTutorialMode = false;
            return;
        }

        isTutorialMode = true;
        tutorialDrawIndex = 0;  // Reset the draw index each time we enable

        // Assign the tutorial data
        tutorialPool = new List<Card>(scenario.tutorialPool);
        tutorialBotDrawSequence = new List<Card>(scenario.tutorialBotDrawSequence);
        tutorialPlayerDrawSequence = new List<Card>(scenario.tutorialPlayerDrawSequence);

        // Override current pool with the tutorial pool
        currentPool = new List<Card>(tutorialPool);

        Debug.Log($"Tutorial Mode Enabled: Scenario {scenarioIndex}");
    }

    /// <summary>
    /// Draws cards from the fixed tutorial sequences instead of randomly.
    /// Each call to this method returns the next pair of (player, bot) cards.
    /// </summary>
    private void DrawTutorialCards(out Card drawnPlayerCard, out Card drawnBotCard)
    {
        if (tutorialDrawIndex >= tutorialBotDrawSequence.Count ||
            tutorialDrawIndex >= tutorialPlayerDrawSequence.Count)
        {
            Debug.LogWarning("No more tutorial draws are defined for this scenario!");
            drawnPlayerCard = null;
            drawnBotCard = null;
            return;
        }

        drawnBotCard = tutorialBotDrawSequence[tutorialDrawIndex];
        drawnPlayerCard = tutorialPlayerDrawSequence[tutorialDrawIndex];
        tutorialDrawIndex++;
    }
}
