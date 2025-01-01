using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class persistentGM : MonoBehaviour
{
    public static persistentGM Instance { get; private set; }

    // The queue that will hold the next 3 playerCounts in random order
    public static Queue<int> playerCountQueue = new Queue<int>();
    // A queue to hold the next 5 boolean outcomes
    public static Queue<bool> boolQueue = new Queue<bool>();
    public static bool isChosenRound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
