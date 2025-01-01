using System.Collections.Generic;
using UnityEngine;

public class NewCardPoolManager : MonoBehaviour
{
    [System.Serializable]
    public struct PoolGroup
    {
        public int peopleCount; // Number of people this group is for
        public List<Pool> pools; // List of pools for this people count
    }

    [SerializeField] private List<PoolGroup> poolGroups; // Configurable pool groups in the Inspector

    private Dictionary<int, (List<Pool>, HashSet<int>)> peoplePools; // Dictionary to store pools and selected indices
    private readonly string[] suits = { "Heart", "Club", "Diamond", "Spade" }; // Available suits

    private void Awake()
    {
        InitializePeoplePools();
    }

    /// <summary>
    /// Initializes the peoplePools dictionary from the serialized poolGroups.
    /// </summary>
    private void InitializePeoplePools()
    {
        peoplePools = new Dictionary<int, (List<Pool>, HashSet<int>)>();

        foreach (var group in poolGroups)
        {
            if (!peoplePools.ContainsKey(group.peopleCount))
            {
                peoplePools[group.peopleCount] = (group.pools, new HashSet<int>());
            }
        }
    }

    /// <summary>
    /// Get a random pool for the specified number of people.
    /// </summary>
    /// <param name="peopleCount">Number of people for the pool.</param>
    /// <returns>List of Cards in the pool.</returns>
    public List<Card> GetRandomPoolForPeople(int peopleCount)
    {
        if (!peoplePools.ContainsKey(peopleCount))
        {
            Debug.LogWarning($"No pool defined for {peopleCount} people.");
            return null;
        }

        var (poolList, selectedPools) = peoplePools[peopleCount];
        return GetRandomPool(poolList, selectedPools);
    }

    /// <summary>
    /// Resets the pools for the specified number of people.
    /// </summary>
    /// <param name="peopleCount">Number of people to reset pools for.</param>
    public void ResetPoolsForPeople(int peopleCount)
    {
        if (peoplePools.ContainsKey(peopleCount))
        {
            peoplePools[peopleCount].Item2.Clear(); // Clear the selected pool indices
        }
    }

    /// <summary>
    /// Generic method to get a random pool and assign unique suits to cards.
    /// </summary>
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

    /// <summary>
    /// Assign unique suits to cards in the pool.
    /// </summary>
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
}
