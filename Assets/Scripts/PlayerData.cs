// Class to store player data
[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int gems;
    public string profileImagePath;
    public bool isTutorial = true;
    public int tutorialRound;

    // field to track total number of games started
    public int gamesPlayed;
}