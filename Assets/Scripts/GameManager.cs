using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CardPoolManager cardPoolManager;
    [SerializeField] private UIManager UIManager;
    [SerializeField] private int playerCount = 4; // 4 (1 player, 3 bots) / 6 (1 player, 5 bots)

    [SerializeField] private GameObject cardPrefab;           // Prefab for the card
    [SerializeField] private Transform cardPoolDisplayParent; // Parent object to display cards

    [Header("Tutorial Settings")]
    [SerializeField]
    private TutorialScenarioFactory tutorialScenarioFactory;  // Tutorail pre-setup

    private Round currentRound;    // Current round instance
    private List<int> bots;        // Bot list (each bot is represented as an ID)
    private int botCount = 0;
    private int playerChips;
    private int currentBet = 50;
    private bool gameOver = false;

    private Card playerCard;       // Player's drawn card for the current round
    private Card botCard;          // Bot's drawn card for the current round

    private string saveFilePath;   // Path for saving and loading player data
    private PlayerData playerData; // Loaded player data

    void Start()
    {
        // save file path
        saveFilePath = Application.persistentDataPath + "/playerData.json";

        // 1) Load player data from file
        playerData = LoadPlayerData();

        // 2) Check if we are in tutorial mode
        if (playerData != null && playerData.isTutorial)
        {
            // If tutorialRound is > 3, tutorial is finished, so switch to normal mode
            if (playerData.tutorialRound > 3)
            {
                playerData.isTutorial = false;
                SavePlayerData(playerData);
                StartGame();
            }
            else
            {
                // Start the tutorial scenario indicated by tutorialRound
                StartTutorialRound(playerData.tutorialRound);
            }
        }
        else
        {
            // round check
            persistentGM.isChosenRound = GetIsChosenRound();
            // Normal game
            StartGame();
        }

        UIManager.UpdatePlayerName(playerData.playerName);
        
        UIManager.AddLeaveButtonListener(() => StartCoroutine(ReturnToEnterScene(true, 0)));
    }

    void changePlayerNumber()
    {

        playerData.gamesPlayed++;

        // If no more randomized counts in the queue, refill it
        if (persistentGM.playerCountQueue.Count == 0)
        {
            // Build a new block: 2x(4 players), 1x(6 players)
            List<int> block = new List<int> { 4, 4, 6 };

            if (!playerData.isTutorial) 
            {
                // Shuffle the block
                Shuffle(block);
            }

            // Enqueue each element from the shuffled block
            foreach (var count in block) persistentGM.playerCountQueue.Enqueue(count);
        }

        // Dequeue the next playerCount from our queue
        playerCount = persistentGM.playerCountQueue.Dequeue();


        Debug.Log("PlayerCount: " + playerData.gamesPlayed + ", " + playerCount);
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            // Swap
            T temp = list[i];
            list[i] = list[r];
            list[r] = temp;
        }
    }

    // ------------------------------------------------------------------------
    // TUTORIAL FLOW
    // ------------------------------------------------------------------------
    private void StartTutorialRound(int scenarioIndex)
    {
        Debug.Log($"Starting tutorial scenario {scenarioIndex}...");

        changePlayerNumber();

        // Reset the game state for one tutorial round
        currentBet = 50;
        playerChips = currentBet;
        gameOver = false;
        botCount = 0;

        // Initialize bots
        bots = new List<int>();
        for (int i = 0; i < playerCount - 1; i++)
        {
            bots.Add(i); // Add bots as IDs (0, 1, 2, etc.)
            botCount += 1;
        }

        // Initialize the round in tutorial mode
        currentRound = new Round(cardPoolManager, playerCount);
        currentRound.EnableTutorialMode(tutorialScenarioFactory, scenarioIndex);  // <--- fixed scenario
        currentRound.RefreshPool();                      // no-op in tutorial mode

        Debug.Log($"Tutorial scenario {scenarioIndex} pool created.");

        // Display cards for 3 seconds and then hide
        StartCoroutine(DisplayCardPoolForLimitedTime());

        // UI updates
        UIManager.UpdateChips(currentBet, false);
        UIManager.UpdateChips(playerChips, true);
        UIManager.UpdatePlayerCard(-1);
        UIManager.UpdateBotCard(-1);
        UIManager.UpdateRoundStatus($"Tutorial Round {scenarioIndex}: Draw your cards.");
        UIManager.SetRevealButtonInteractable(true);
        UIManager.HideResultImages();

        // The reveal button calls the same method as normal
        UIManager.AddRevealButtonListener(RevealBotCard);

        // Show the current bot’s win count
        if (bots.Count > 0)
        {

            UIManager.UpdateBotName(bots[0]+1);
            UIManager.UpdateBotWinCount(bots[0], botCount);
        }

        // Automatically draw cards at the start of the round
        DrawCards();
    }

    // ------------------------------------------------------------------------
    // NORMAL FLOW
    // ------------------------------------------------------------------------
    private void StartGame()
    {
        changePlayerNumber();

        // Reset the game state
        currentBet = 50;
        playerChips = currentBet;
        gameOver = false;
        botCount = 0;

        // Initialize bots
        bots = new List<int>();
        for (int i = 0; i < playerCount - 1; i++)
        {
            bots.Add(i);
            botCount++;
        }

        // Normal (random) round
        currentRound = new Round(cardPoolManager, playerCount);
        currentRound.RefreshPool();

        Debug.Log("Game started in normal mode.");

        // Display cards for 3 seconds and then hide
        StartCoroutine(DisplayCardPoolForLimitedTime());

        // UI
        UIManager.UpdateChips(currentBet, false);
        UIManager.UpdateChips(playerChips, true);
        UIManager.UpdatePlayerCard(-1);
        UIManager.UpdateBotCard(-1);
        UIManager.UpdateRoundStatus("Game started! Draw your cards.");
        UIManager.SetRevealButtonInteractable(true);
        UIManager.HideResultImages();
        UIManager.AddRevealButtonListener(RevealBotCard);

        if (bots.Count > 0)
        {

            UIManager.UpdateBotName(bots[0]+1);
            UIManager.UpdateBotWinCount(bots[0], botCount);
        }

        // Draw cards
        DrawCards();
    }

    // ------------------------------------------------------------------------
    // SHARED ROUND LOGIC
    // ------------------------------------------------------------------------
    private void DrawCards()
    {
        if (gameOver)
        {
            Debug.LogWarning("Game is over. Cannot draw cards.");
            return;
        }

        if(persistentGM.isChosenRound) { Debug.LogWarning("Chosen black bean card"); }
        currentRound.DrawCards(out playerCard, out botCard, persistentGM.isChosenRound);

        UIManager.UpdatePlayerCard(playerCard.value, playerCard.suit);
        UIManager.UpdateBotCard(-1); // Hide bot card in the UI

        if (playerCard == null || botCard == null)
        {
            StartCoroutine(ReturnToEnterScene(true, 0));
        }
    }

    private bool GetIsChosenRound()
    {
        if (persistentGM.boolQueue.Count == 0)
        {
            // Build a block with 1 true and 4 false
            List<bool> block = new List<bool> { true, false, false, false, false };

            // Shuffle the block using Fisher-Yates
            Shuffle(block);

            // Enqueue each element from the shuffled block
            foreach (var b in block)
            {
                persistentGM.boolQueue.Enqueue(b);
            }
        }

        // Dequeue the next boolean
        return persistentGM.boolQueue.Dequeue();
    }

    private void RevealBotCard()
    {
        if (gameOver || bots.Count == 0)
        {
            Debug.LogWarning("Cannot play round. Game is over.");
            return;
        }

        // Reveal the bot's card
        UIManager.UpdateBotCard(botCard.value, botCard.suit);
        Debug.Log($"Player card: {playerCard.value} of {playerCard.suit}, Bot card: {botCard.value} of {botCard.suit}");

        // Play animation and subscribe to the completion event
        UIManager.SubscribeToAnimationCompleted(OnAnimationCompleted);
        UIManager.PlayCardRevealAnimation();
    }

    private void OnAnimationCompleted()
    {
        // Unsubscribe from the event to avoid multiple triggers
        UIManager.UnsubscribeFromAnimationCompleted(OnAnimationCompleted);

        // Compare cards
        if (playerCard.value >= botCard.value)
        {
            // Player wins
            Debug.Log("Player wins this round!");
            UIManager.UpdateRoundStatus("Player wins this round!");
            UIManager.ShowWinImage();

            // Remove the current bot
            bots.RemoveAt(0);
            playerChips += currentBet;
            currentBet *= 2;

            UIManager.UpdateChips(currentBet, false);
            UIManager.UpdateChips(playerChips, true);

            if (bots.Count > 0)
            {
                UIManager.UpdateBotName(bots[0] + 1);
                UIManager.UpdateBotWinCount(bots[0], botCount);
            }

            // If no bots left, the round is fully won
            if (bots.Count == 0)
            {
                Debug.Log("All bots defeated! Player wins the game!");
                UIManager.UpdateRoundStatus("All bots defeated! You win!");
                UIManager.SetRevealButtonInteractable(false);
                gameOver = true;
                nextTutorial();
                SavePlayerData(playerData);
                // Normal logic: Return to enter scene or show a restart
                StartCoroutine(ReturnToEnterScene(true));
                return;
            }
        }
        else
        {
            // Player loses
            Debug.Log("Player loses this round.");
            UIManager.UpdateRoundStatus("Player loses!");
            UIManager.ShowLoseImage();
            nextTutorial();
            SavePlayerData(playerData);
            // Normal logic: Return to enter scene or show a restart
            StartCoroutine(ReturnToEnterScene(false));
            return;
        }

        // If the pool is empty, refresh
        if (currentRound.GetCurrentPool().Count == 0)
        {
            Debug.Log("Current pool is empty. Updating pool...");
            currentRound.RefreshPool();
            DisplayCardPool();
            StartCoroutine(DisplayCardPoolForLimitedTime());
        }

        // Start next round after a delay
        StartCoroutine(NextRoundWithDelay());
    }

    void nextTutorial()
    {
        // Check if in a tutorial round
        if (playerData != null && playerData.isTutorial)
        {
            // Move to next tutorial scenario
            playerData.tutorialRound++;
            // If passed scenario 3, end the tutorial
            if (playerData.tutorialRound > 3)
            {
                playerData.isTutorial = false;
                Debug.Log("Tutorial completed!");
            }
        }
    }

    // ------------------------------------------------------------------------
    // UI + Flow Helpers
    // ------------------------------------------------------------------------
    private void DisplayCardPool()
    {
        foreach (Transform child in cardPoolDisplayParent)
        {
            Destroy(child.gameObject);
        }

        string cards = "";
        string suits = "";

        foreach (Card card in currentRound.GetCurrentPool())
        {
            GameObject cardInstance = Instantiate(cardPrefab, cardPoolDisplayParent);
            CardUI cardUI = cardInstance.GetComponent<CardUI>();
            cardUI.SetCard(card.value, card.suit);
            cards += card.value + ", ";
            suits += card.suit + ", ";
        }

        Debug.Log("Card Values in Pool: " + cards);
        Debug.Log("Card Suits in Pool:  " + suits);
    }

    IEnumerator DisplayCardPoolForLimitedTime()
    {
        cardPoolDisplayParent.gameObject.SetActive(true);
        UIManager.ChangeBotCountStatus(false);

        DisplayCardPool(); // Show the pool

        yield return new WaitForSeconds(3f);

        cardPoolDisplayParent.gameObject.SetActive(false);
        UIManager.ChangeBotCountStatus(true);
    }

    IEnumerator NextRoundWithDelay()
    {
        UIManager.SetRevealButtonInteractable(false);

        yield return new WaitForSeconds(2f);
        UIManager.HideResultImages();
        DrawCards();
        UIManager.SetRevealButtonInteractable(true);
    }

    IEnumerator RestartGameWithDelay()
    {
        UIManager.SetRevealButtonInteractable(false);

        yield return new WaitForSeconds(2f);
        UIManager.HideResultImages();
        StartGame();
    }

    // ------------------------------------------------------------------------
    // RETURN / LEAVE
    // ------------------------------------------------------------------------
    IEnumerator ReturnToEnterScene(bool playerLeave, float waitSecond=2f)
    {
        UpdatePlayerData(playerLeave);

        yield return new WaitForSeconds(waitSecond);

        SceneManager.LoadScene("Enter");
    }

    private void UpdatePlayerData(bool playerLeave)
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            if (playerLeave)
            {
                if (currentBet > 50)
                {
                    UIManager.ShowWinResult();
                    data.gems += playerChips;
                }
            }
            else
            {
                UIManager.ShowLoseResult();
                data.gems -= 50;
            }

            // Save updated
            string updatedJson = JsonUtility.ToJson(data, true);
            File.WriteAllText(saveFilePath, updatedJson);
        }
    }

    // ------------------------------------------------------------------------
    // LOAD / SAVE
    // ------------------------------------------------------------------------
    private PlayerData LoadPlayerData()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogError("playerData.json not found");
        }

        string json = File.ReadAllText(saveFilePath);
        return JsonUtility.FromJson<PlayerData>(json);
    }

    private void SavePlayerData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
    }
}
