using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Card
{
    public int value;
    public string suit;
    public Card(int value, string suit)
    {
        this.value = value;
        this.suit = suit;
    }

    /// <summary>
    /// !Warning: Deprecated methods
    /// </summary>
    /// <returns></returns>
    public bool IsBlackBean()
    {
        return value >= 7;
    }

    public bool IsBlackBean(Pool pool)
    {
        if(pool == null || pool.cards == null || pool.cards.Count <= 2)
        {
            return false;
        }

        // Check if the card actually exists in the pool 
        // (match by value)
        // TODO: Add suit match
        if (!pool.cards.Any(c => c.value == value))
        {
            return false;
        }

        // Sort the cards in the pool by their value
        var sortedCards = pool.cards.OrderBy(c => c.value).ToList();

        // Find the index of the specified card in the sorted list
        int indexOfCard = sortedCards.FindIndex(c => c.value == value && c.suit == suit);

        // The number of cards with a strictly lower value is the index in the sorted list
        int numLess = indexOfCard;

        // Opponent can choose from (pool.Count - 1) cards
        int possibleOpponentCards = pool.cards.Count - 1;

        // Calculate win probability
        double probability = (double)numLess / possibleOpponentCards;

        return probability >= 0.6;
    }

    public bool IsBlackBean(List<Card> cards)
    {
        if (cards == null || cards.Count <= 2)
        {
            return false;
        }

        // Check if the card actually exists in the pool 
        // (match by value)
        // TODO: Add suit match
        if (!cards.Any(c => c.value == value))
        {
            return false;
        }

        // Sort the cards in the pool by their value
        var sortedCards = cards.OrderBy(c => c.value).ToList();

        // Find the index of the specified card in the sorted list
        int indexOfCard = sortedCards.FindIndex(c => c.value == value && c.suit == suit);

        // The number of cards with a strictly lower value is the index in the sorted list
        int numLess = indexOfCard;

        // Opponent can choose from (pool.Count - 1) cards
        int possibleOpponentCards = cards.Count - 1;

        // Calculate win probability
        double probability = (double)numLess / possibleOpponentCards;

        return probability >= 0.6;
    }
}

// class for each sub-pool
[System.Serializable]
public class Pool
{
    public List<Card> cards;
}

[System.Obsolete("This class is deprecated and should no longer be used. Use NewCardPoolManager instead.", false)]
public class CardPoolManager : MonoBehaviour
{
    [SerializeField] private List<Pool> threePeopleList; // List of pools for three-player games
    [SerializeField] private List<Pool> fivePeopleList;  // List of pools for five-player games

    private HashSet<int> selectedThreePeoplePools = new HashSet<int>(); // Selected sub-pools in threePeopleList
    private HashSet<int> selectedFivePeoplePools = new HashSet<int>(); // Selected sub-pools in fivePeopleList

    private readonly string[] suits = { "Heart", "Club", "Diamond", "Spade" }; // Available suits

    // Public method to get a random pool for three people
    public List<Card> GetRandomThreePeoplePool()
    {
        return GetRandomPool(threePeopleList, selectedThreePeoplePools);
    }

    // Public method to get a random pool for five people
    public List<Card> GetRandomFivePeoplePool()
    {
        return GetRandomPool(fivePeopleList, selectedFivePeoplePools);
    }

    // Generic method to get a random pool and assign unique suits to cards
    private List<Card> GetRandomPool(List<Pool> poolList, HashSet<int> selectedPools)
    {
        // Ensure all pools are selectable again if all have been selected
        if (selectedPools.Count == poolList.Count)
        {
            selectedPools.Clear(); // Reset selection for pools
        }

        // Find available pools (those not yet selected)
        List<int> availablePools = new List<int>();
        for (int i = 0; i < poolList.Count; i++)
        {
            if (!selectedPools.Contains(i))
                availablePools.Add(i);
        }

        // If no available pools, return null (this shouldn't happen after reset logic)
        if (availablePools.Count == 0)
        {
            Debug.LogWarning("No available pools to select!");
            return null;
        }

        // Randomly select a pool index from the available ones
        int selectedIndex = availablePools[Random.Range(0, availablePools.Count)];
        selectedPools.Add(selectedIndex); // Mark this pool as selected

        // Assign unique suits to the cards in the pool
        List<Card> cards = poolList[selectedIndex].cards;
        AssignUniqueSuits(cards);

        return cards;
    }

    // Assign unique suits to cards in the pool
    private void AssignUniqueSuits(List<Card> cards)
    {
        Dictionary<int, HashSet<string>> usedSuits = new Dictionary<int, HashSet<string>>();

        foreach (Card card in cards)
        {
            if (!usedSuits.ContainsKey(card.value))
            {
                usedSuits[card.value] = new HashSet<string>();
            }

            // Find an unused suit for this card value
            foreach (string suit in suits)
            {
                if (!usedSuits[card.value].Contains(suit))
                {
                    card.suit = suit;
                    usedSuits[card.value].Add(suit);
                    break;
                }
            }
        }
    }

    // Manual clear methods
    public void ResetThreePeoplePools()
    {
        selectedThreePeoplePools.Clear();
    }

    public void ResetFivePeoplePools()
    {
        selectedFivePeoplePools.Clear();
    }
}