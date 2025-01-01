using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

public class EnterUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image profileImage;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI gemCountText;
    public Button betButton;
    public Button quitButton;

    [Header("Player Data")]
    private PlayerData playerData; // Stores player data

    private string saveFilePath;

    private void Start()
    {
        // Initialize the save file path
        saveFilePath = Application.persistentDataPath + "/playerData.json";

        // Load player data
        LoadPlayerData();

        // Update the UI with the loaded data
        UpdateUI();

        // Add button listeners
        betButton.onClick.AddListener(() => LoadMainGameScene());
        quitButton.onClick.AddListener(() => QuitGame());
    }

    private void LoadPlayerData()
    {
        // Check if save file exists
        if (File.Exists(saveFilePath))
        {
            Debug.Log("Load");
            string json = File.ReadAllText(saveFilePath);
            playerData = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            Debug.Log("Create");
            // Create default player data if no save file exists
            playerData = new PlayerData
            {
                playerName = "Player",
                gems = 100,
                profileImagePath = "",
                isTutorial = true,
                tutorialRound = 1,
                gamesPlayed = 0
            };
            SavePlayerData();
        }
    }

    private void SavePlayerData()
    {
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(saveFilePath, json);
    }

    private void UpdateUI()
    {
        if (playerData != null)
        {
            playerNameText.text = playerData.playerName;
            gemCountText.text = playerData.gems.ToString();

            // Load and set profile image if the path exists
            if (!string.IsNullOrEmpty(playerData.profileImagePath) && File.Exists(playerData.profileImagePath))
            {
                byte[] imageData = File.ReadAllBytes(playerData.profileImagePath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageData);
                profileImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }
    }

    private void LoadMainGameScene()
    {
        SceneManager.LoadScene("MainGame");
    }

    private void QuitGame()
    {
        SavePlayerData();

        Application.Quit();

        // For testing in the editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}